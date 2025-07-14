using Zypher.Notifications.Messages;

namespace Zypher.Notifications.Interfaces;

/// <summary>
/// Handler to orchestrate notifications within the context.
/// </summary>
public interface INotificationHandler
{
    /// <summary>
    /// Stores a single notification in the context.
    /// </summary>
    /// <param name="notification">The notification to store.</param>
    void Notify(Notification notification);

    /// <summary>
    /// Stores a list of notifications in the context.
    /// </summary>
    /// <param name="notifications">The collection of notifications to store.</param>
    void Notify(IEnumerable<Notification> notifications);

    /// <summary>
    /// Retrieves the list of notifications from the context.
    /// </summary>
    /// <returns>A collection of notifications.</returns>
    IEnumerable<Notification> Get();

    /// <summary>
    /// Checks if there are any notifications present in the context.
    /// </summary>
    /// <returns>True if there are notifications; otherwise, false.</returns>
    bool HasNotifications();

    /// <summary>
    /// Clears the list of notifications from the context.
    /// </summary>
    void Clear();
}
