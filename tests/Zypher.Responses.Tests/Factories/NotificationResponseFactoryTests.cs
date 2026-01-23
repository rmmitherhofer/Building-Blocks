using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Notifications.Messages;
using Zypher.Responses.Factories;

namespace Zypher.Responses.Tests.Factories;

public class NotificationResponseFactoryTests
{
    [Fact(DisplayName =
        "Given a notification, " +
        "When ToResponse is called, " +
        "Then it maps all properties")]
    [Trait("Type", nameof(NotificationResponseFactory))]
    public async Task ToResponse_Notification_MapsProperties()
    {
        //Given
        var notification = new Notification(LogLevel.Warning, "type", "key", "value", "detail");

        //When
        var response = notification.ToResponse();

        //Then
        response.Id.Should().Be(notification.Id);
        response.AggregateId.Should().Be(notification.AgregationId);
        response.Type.Should().Be(notification.Type);
        response.LogLevel.Should().Be(notification.LogLevel);
        response.Key.Should().Be(notification.Key);
        response.Value.Should().Be(notification.Value);
        response.Detail.Should().Be(notification.Detail);
        response.Timestamp.Should().Be(notification.Timestamp);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null notification, " +
        "When ToResponse is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(NotificationResponseFactory))]
    public async Task ToResponse_NullNotification_Throws()
    {
        //Given
        Notification? notification = null;

        //When
        Action action = () => notification!.ToResponse();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given notifications, " +
        "When ToResponse is called, " +
        "Then it maps the collection")]
    [Trait("Type", nameof(NotificationResponseFactory))]
    public async Task ToResponse_Collection_MapsAll()
    {
        //Given
        var notifications = new[]
        {
            new Notification(LogLevel.Warning, "type", "key1", "value1"),
            new Notification(LogLevel.Error, "type", "key2", "value2")
        };

        //When
        var responses = notifications.ToResponse().ToList();

        //Then
        responses.Should().HaveCount(2);
        responses[0].Key.Should().Be("key1");
        responses[1].Key.Should().Be("key2");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null collection, " +
        "When ToResponse is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(NotificationResponseFactory))]
    public async Task ToResponse_NullCollection_Throws()
    {
        //Given
        IEnumerable<Notification>? notifications = null;

        //When
        Action action = () => notifications!.ToResponse();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
