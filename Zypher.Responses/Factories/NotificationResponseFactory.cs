using Zypher.Notifications.Messages;

namespace Zypher.Responses.Factories;

/// <summary>
/// Provides factory methods to convert <see cref="Notification"/> domain entities 
/// into <see cref="NotificationResponse"/> response models.
/// </summary>
public static class NotificationResponseFactory
{
    /// <summary>
    /// Converts a <see cref="Notification"/> domain entity into a <see cref="NotificationResponse"/> DTO.
    /// </summary>
    /// <param name="notification">The domain notification to convert.</param>
    /// <returns>A <see cref="NotificationResponse"/> representing the notification.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the notification is null.</exception>
    public static NotificationResponse ToResponse(this Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification, nameof(Notification));

        return new()
        {
            AggregateId = notification.AgregationId,
            LogLevel = notification.LogLevel,
            Type = notification.Type,
            Key = notification.Key,
            Value = notification.Value,
            Detail = notification.Detail,
            Id = notification.Id,
            Timestamp = notification.Timestamp
        };
    }
    /// <summary>
    /// Converts a collection of <see cref="Notification"/> entities to a collection of <see cref="NotificationResponse"/> DTOs.
    /// </summary>
    /// <param name="notifications">The collection of notifications to convert.</param>
    /// <returns>A collection of response DTOs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the notifications collection is null.</exception>

    public static IEnumerable<NotificationResponse> ToResponse(this IEnumerable<Notification> notifications)
    {
        ArgumentNullException.ThrowIfNull(notifications, nameof(IEnumerable<Notification>));

        return notifications.Select(notification => notification.ToResponse());
    }
}
