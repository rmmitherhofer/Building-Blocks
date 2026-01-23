using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Http.Configurations;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Configurations;

public class ExtensionConfigurationsTests
{
    [Fact(DisplayName =
        "Given a service collection, " +
        "When AddHttpConfig is called, " +
        "Then it registers IHttpContextAccessor")]
    [Trait("Type", nameof(ExtensionConfigurations))]
    public async Task AddHttpConfig_RegistersHttpContextAccessor()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddHttpConfig();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<IHttpContextAccessor>().Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app without IHttpContextAccessor, " +
        "When UseHttpConfig is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(ExtensionConfigurations))]
    public async Task UseHttpConfig_WithoutAccessor_Throws()
    {
        //Given
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        Action action = () => app.UseHttpConfig();

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app with IHttpContextAccessor, " +
        "When UseHttpConfig is called, " +
        "Then it configures HttpClient and HttpRequestMessage extensions")]
    [Trait("Type", nameof(ExtensionConfigurations))]
    public async Task UseHttpConfig_ConfiguresExtensions()
    {
        //Given
        var services = new ServiceCollection();
        services.AddHttpConfig();
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        app.UseHttpConfig();

        var client = new HttpClient { BaseAddress = new Uri("https://example.com") };
        client.AddHeaderRequestTemplate("/users/{id}");

        var request = new HttpRequestMessage();
        request.AddHeaderRequestTemplate("/users/{id}");

        //Then
        client.DefaultRequestHeaders.Contains(HttpClientExtensions.X_REQUEST_TEMPLATE).Should().BeTrue();
        request.Headers.Contains(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE).Should().BeTrue();
        await Task.CompletedTask;
    }
}
