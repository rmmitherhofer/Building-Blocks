using Microsoft.Extensions.Logging;

namespace Common.Notifications.Messages;

/// <summary>
/// Represents a domain notification with metadata such as timestamp, log level, and content.
/// </summary>
public class Notification : Message
{
    /// <summary>
    /// Unique identifier of the notification.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Timestamp when the notification was created.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Logging level associated with the notification.
    /// </summary>
    public LogLevel LogLevel { get; private set; }

    /// <summary>
    /// Optional key that identifies the context of the notification.
    /// </summary>
    public string? Key { get; private set; }

    /// <summary>
    /// Main content or value of the notification.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Optional detailed message.
    /// </summary>
    public string Detail { get; private set; }

    /// <summary>
    /// Initializes a new notification with full context.
    /// </summary>
    public Notification(LogLevel logLevel, string type, string key, string value, string detail) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = logLevel;
        Key = key;
        Value = value;
        Detail = detail;
    }

    /// <summary>
    /// Initializes a new notification with a default log level (Information).
    /// </summary>
    public Notification(string type, string key, string value, string detail) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Information;
        Key = key;
        Value = value;
        Detail = detail;
    }

    /// <summary>
    /// Initializes a new notification with specified log level, key and value.
    /// </summary>
    public Notification(LogLevel logLevel, string type, string key, string value) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = logLevel;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Initializes a new notification with default log level (Information), key and value.
    /// </summary>
    public Notification(string type, string key, string value) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Information;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Initializes a new notification with specified log level, key and value.
    /// </summary>
    public Notification(LogLevel logLevel, string key, string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        LogLevel = logLevel;
        Timestamp = DateTime.Now;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Initializes a new notification with default log level (Information), key and value.
    /// </summary>
    public Notification(string key, string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        LogLevel = LogLevel.Information;
        Timestamp = DateTime.Now;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Initializes a new notification with specified log level and value only.
    /// </summary>
    public Notification(LogLevel logLevel, string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        LogLevel = logLevel;
        Timestamp = DateTime.Now;
        Value = value;
    }

    /// <summary>
    /// Initializes a new notification with default log level (Information) and value only.
    /// </summary>
    public Notification(string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Information;
        Value = value;
    }
}
