using System;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Xunit;
using Zypher.Api.Foundation.Configurations;
using Zypher.Notifications.Interfaces;

namespace Zypher.Api.Foundation.Tests.Configurations;

public class ApiConfigurationTests
{
    private sealed class FakeEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "test";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = System.IO.Directory.GetCurrentDirectory();
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = System.IO.Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact(DisplayName =
        "Given a null service collection, " +
        "When AddZypherApiFoundation is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(ApiConfiguration))]
    public async Task AddZypherApiFoundation_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;
        var configuration = new ConfigurationManager();
        var environment = new FakeEnvironment();

        //When
        Action action = () => services!.AddZypherApiFoundation(configuration, environment);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null configuration, " +
        "When AddZypherApiFoundation is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(ApiConfiguration))]
    public async Task AddZypherApiFoundation_NullConfiguration_Throws()
    {
        //Given
        var services = new ServiceCollection();
        IConfiguration? configuration = null;
        var environment = new FakeEnvironment();

        //When
        Action action = () => services.AddZypherApiFoundation(configuration!, environment);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given valid inputs, " +
        "When AddZypherApiFoundation is called, " +
        "Then it registers notification services")]
    [Trait("Type", nameof(ApiConfiguration))]
    public async Task AddZypherApiFoundation_RegistersNotificationHandler()
    {
        //Given
        var services = new ServiceCollection();
        var configuration = new ConfigurationManager();
        var environment = new FakeEnvironment();

        //When
        services.AddZypherApiFoundation(configuration, environment);

        //Then
        services.Should().Contain(x => x.ServiceType == typeof(INotificationHandler));
        services.Should().Contain(x => x.ServiceType == typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
        await Task.CompletedTask;
    }
}
