using FluentAssertions;
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
        services.AddNotification();
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
        Action action = () => services!.AddNotification();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
