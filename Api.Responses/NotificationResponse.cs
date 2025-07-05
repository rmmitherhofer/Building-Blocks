using Microsoft.Extensions.Logging;

namespace Api.Responses;

/// <summary>
/// Represents a notification message returned by the API, typically used for logging or audit purposes.
/// </summary>
public class NotificationResponse : MessageResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the notification was generated.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the log level that describes the severity of the notification.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets an optional key used to categorize or identify the message.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the main message value or content.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets additional detailed information associated with the notification.
    /// </summary>
    public string Detail { get; set; }
}
