using System;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Notifications.Configurations;
using Zypher.Notifications.Handlers;
using Zypher.Notifications.Interfaces;

namespace Zypher.Notifications.Tests.Configurations;

public class NotificationConfigurationTests
{
    [Fact(DisplayName =
        "Given a service collection, " +
        "When AddNotificationConfig is called, " +
        "Then it registers notifications and logging")]
    [Trait("Type", nameof(NotificationConfiguration))]
    public async Task AddNotificationConfig_RegistersServices()
    {
        //Given
        var services = new ServiceCollection();
        services.AddLogging();

        //When
        services.AddNotificationConfig();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<IHttpContextAccessor>().Should().NotBeNull();
        provider.GetService<INotificationHandler>().Should().BeOfType<NotificationHandler>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null service collection, " +
        "When AddNotificationConfig is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(NotificationConfiguration))]
    public async Task AddNotificationConfig_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;

        //When
        Action action = () => services!.AddNotificationConfig();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app without accessor, " +
        "When UseNotificationConfig is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(NotificationConfiguration))]
    public async Task UseNotificationConfig_WithoutAccessor_Throws()
    {
        //Given
        var provider = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        Action action = () => app.UseNotificationConfig();

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null application builder, " +
        "When UseNotificationConfig is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(NotificationConfiguration))]
    public async Task UseNotificationConfig_NullApp_Throws()
    {
        //Given
        IApplicationBuilder? app = null;

        //When
        Action action = () => app!.UseNotificationConfig();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an app with accessor, " +
        "When UseNotificationConfig is called, " +
        "Then it returns the same app")]
    [Trait("Type", nameof(NotificationConfiguration))]
    public async Task UseNotificationConfig_WithAccessor_ReturnsSameApp()
    {
        //Given
        var services = new ServiceCollection();
        services.AddNotificationConfig();
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        //When
        var result = app.UseNotificationConfig();

        //Then
        result.Should().BeSameAs(app);
        await Task.CompletedTask;
    }
}
