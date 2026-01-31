using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Notifications.Handlers;
using Zypher.Notifications.Messages;

namespace Zypher.Notifications.Tests.Handlers;

public class NotificationHandlerTests
{
    private sealed class TestLogger : ILogger<NotificationHandler>
    {
        public sealed record Entry(LogLevel Level, string Message);

        public List<Entry> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new Entry(logLevel, formatter(state, exception)));
        }
    }

    private static NotificationHandler CreateHandler(HttpContext? context, out TestLogger logger)
    {
        var accessor = new HttpContextAccessor { HttpContext = context };
        logger = new TestLogger();
        return new NotificationHandler(accessor, logger);
    }

    [Fact(DisplayName =
        "Given a warning notification, " +
        "When Notify is called, " +
        "Then it stores the notification and logs it")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task Notify_Single_AddsAndLogsWarning()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out var logger);
        var notification = new Notification(LogLevel.Warning, "type", "code", "value");

        //When
        handler.Notify(notification);

        //Then
        handler.Get().Should().ContainSingle().Which.Should().Be(notification);
        logger.Entries.Should().ContainSingle();
        logger.Entries[0].Level.Should().Be(LogLevel.Warning);
        logger.Entries[0].Message.Should().Contain("[Notification]|code: value");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an information notification, " +
        "When Notify is called, " +
        "Then it stores the notification without logging")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task Notify_Single_Info_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out var logger);
        var notification = new Notification(LogLevel.Information, "type", "code", "value");

        //When
        handler.Notify(notification);

        //Then
        handler.Get().Should().ContainSingle();
        logger.Entries.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given multiple notifications, " +
        "When Notify is called, " +
        "Then it stores all and logs for warning or higher")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task Notify_Multiple_AddsAllAndLogs()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out var logger);
        var notifications = new[]
        {
            new Notification(LogLevel.Information, "type", "k1", "v1"),
            new Notification(LogLevel.Warning, "type", "k2", "v2"),
            new Notification(LogLevel.Error, "type", "k3", "v3"),
            new Notification(LogLevel.Critical, "type", "k4", "v4")
        };

        //When
        handler.Notify(notifications);

        //Then
        handler.Get().Should().HaveCount(4);
        logger.Entries.Should().HaveCount(3);
        logger.Entries.Should().ContainSingle(x => x.Level == LogLevel.Warning);
        logger.Entries.Should().ContainSingle(x => x.Level == LogLevel.Error);
        logger.Entries.Should().ContainSingle(x => x.Level == LogLevel.Critical);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a handler without context, " +
        "When Notify is called, " +
        "Then it does nothing")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task Notify_NoContext_NoOp()
    {
        //Given
        var handler = CreateHandler(null, out var logger);
        var notification = new Notification(LogLevel.Warning, "type", "code", "value");

        //When
        handler.Notify(notification);

        //Then
        handler.Get().Should().BeEmpty();
        logger.Entries.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given notifications below warning, " +
        "When HasNotifications is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task HasNotifications_InformationOnly_ReturnsFalse()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out _);
        handler.Notify(new Notification(LogLevel.Information, "type", "k", "v"));

        //When
        var result = handler.HasNotifications();

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a warning notification, " +
        "When HasNotifications is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task HasNotifications_Warning_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out _);
        handler.Notify(new Notification(LogLevel.Warning, "type", "k", "v"));

        //When
        var result = handler.HasNotifications();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored notifications, " +
        "When Clear is called, " +
        "Then it removes them from context")]
    [Trait("Type", nameof(NotificationHandler))]
    public async Task Clear_RemovesNotifications()
    {
        //Given
        var context = new DefaultHttpContext();
        var handler = CreateHandler(context, out _);
        handler.Notify(new Notification(LogLevel.Warning, "type", "k", "v"));

        //When
        handler.Clear();

        //Then
        handler.Get().Should().BeEmpty();
        await Task.CompletedTask;
    }
}
