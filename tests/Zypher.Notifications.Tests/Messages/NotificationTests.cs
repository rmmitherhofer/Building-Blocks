using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Notifications.Messages;

namespace Zypher.Notifications.Tests.Messages;

public class NotificationTests
{
    [Fact(DisplayName =
        "Given full notification data, " +
        "When Notification is created, " +
        "Then it sets all properties")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_FullConstructor_SetsProperties()
    {
        //Given
        var now = DateTime.Now;

        //When
        var notification = new Notification(LogLevel.Warning, "type", "key", "value", "detail");

        //Then
        notification.Id.Should().NotBeEmpty();
        notification.AgregationId.Should().Be(notification.Id);
        notification.Type.Should().Be("type");
        notification.Key.Should().Be("key");
        notification.Value.Should().Be("value");
        notification.Detail.Should().Be("detail");
        notification.LogLevel.Should().Be(LogLevel.Warning);
        notification.Timestamp.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a notification without log level, " +
        "When it is created, " +
        "Then it defaults to Information")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_DefaultLogLevel_IsInformation()
    {
        //Given
        var now = DateTime.Now;

        //When
        var notification = new Notification("type", "key", "value", "detail");

        //Then
        notification.LogLevel.Should().Be(LogLevel.Information);
        notification.Timestamp.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a key and value, " +
        "When Notification is created, " +
        "Then it uses Notification as type")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_KeyValue_UsesDefaultType()
    {
        //Given
        var notification = new Notification("key", "value");

        //When
        var type = notification.Type;

        //Then
        type.Should().Be(nameof(Notification));
        notification.Key.Should().Be("key");
        notification.Value.Should().Be("value");
        notification.LogLevel.Should().Be(LogLevel.Information);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a log level with key and value, " +
        "When Notification is created, " +
        "Then it uses Notification as type and preserves log level")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_LogLevelKeyValue_UsesDefaultType()
    {
        //Given
        var notification = new Notification(LogLevel.Error, "key", "value");

        //When
        var type = notification.Type;

        //Then
        type.Should().Be(nameof(Notification));
        notification.Key.Should().Be("key");
        notification.Value.Should().Be("value");
        notification.LogLevel.Should().Be(LogLevel.Error);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value only, " +
        "When Notification is created, " +
        "Then it uses Notification as type")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_ValueOnly_UsesDefaultType()
    {
        //Given
        var notification = new Notification("value");

        //When
        var type = notification.Type;

        //Then
        type.Should().Be(nameof(Notification));
        notification.Value.Should().Be("value");
        notification.LogLevel.Should().Be(LogLevel.Information);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a log level and value, " +
        "When Notification is created, " +
        "Then it uses Notification as type and preserves log level")]
    [Trait("Type", nameof(Notification))]
    public async Task Notification_LogLevelValue_UsesDefaultType()
    {
        //Given
        var notification = new Notification(LogLevel.Critical, "value");

        //When
        var type = notification.Type;

        //Then
        type.Should().Be(nameof(Notification));
        notification.Value.Should().Be("value");
        notification.LogLevel.Should().Be(LogLevel.Critical);
        await Task.CompletedTask;
    }
}
