using System;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Logs.Configurations;
using Zypher.Logs.Extensions;

namespace Zypher.Logs.Tests.Configurations;

public class LogConfigurationsTests
{
    [Fact(DisplayName =
        "Given a service collection, " +
        "When AddConsoleLogExtensionConfig is called, " +
        "Then it registers IHttpContextAccessor")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task AddConsoleLogExtensionConfig_RegistersAccessor()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddConsoleLogExtensionConfig();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<IHttpContextAccessor>().Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null service collection, " +
        "When AddConsoleLogExtensionConfig is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task AddConsoleLogExtensionConfig_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;

        //When
        Action action = () => services!.AddConsoleLogExtensionConfig();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app without accessor, " +
        "When UseConsoleLogExtensionConfig is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task UseConsoleLogExtensionConfig_WithoutAccessor_Throws()
    {
        //Given
        var provider = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        Action action = () => app.UseConsoleLogExtensionConfig();

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null application builder, " +
        "When UseConsoleLogExtensionConfig is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task UseConsoleLogExtensionConfig_NullApp_Throws()
    {
        //Given
        IApplicationBuilder? app = null;

        //When
        Action action = () => app!.UseConsoleLogExtensionConfig();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app with accessor, " +
        "When UseConsoleLogExtensionConfig is called, " +
        "Then it configures ConsoleLogExtensions")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task UseConsoleLogExtensionConfig_ConfiguresExtensions()
    {
        //Given
        var services = new ServiceCollection();
        services.AddConsoleLogExtensionConfig();
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        app.UseConsoleLogExtensionConfig();
        var logger = new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger("test");

        //Then
        logger.Invoking(l => l.LogInfo("message")).Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an application builder, " +
        "When UseLogDecoratorConfig is called twice, " +
        "Then it registers middleware only once")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task UseLogDecoratorConfig_Idempotent()
    {
        //Given
        var provider = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        var key = typeof(Zypher.Logs.Middlewares.LogDecoratorMiddleware).FullName!;

        //When
        app.UseLogDecoratorConfig();
        app.UseLogDecoratorConfig();

        //Then
        app.Properties.ContainsKey(key).Should().BeTrue();
        app.Properties[key].Should().Be(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null application builder, " +
        "When UseLogDecoratorConfig is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(LogConfigurations))]
    public async Task UseLogDecoratorConfig_NullApp_Throws()
    {
        //Given
        IApplicationBuilder? app = null;

        //When
        Action action = () => app!.UseLogDecoratorConfig();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
