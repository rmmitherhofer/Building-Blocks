using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Notifications.Handlers;

/// <summary>
/// Handles application notifications by storing them in the current HTTP context and logging them.
/// </summary>
public class NotificationHandler : INotificationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<NotificationHandler> _logger;
    private const string NotificationsKey = "__Notifications__";

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="logger">The logger instance.</param>
    public NotificationHandler(IHttpContextAccessor httpContextAccessor, ILogger<NotificationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Adds a single notification to the current HTTP context and logs it.
    /// </summary>
    /// <param name="notification">The notification to add.</param>
    public void Notify(Notification notification)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        if (!context.Items.TryGetValue(NotificationsKey, out var obj) || obj is not List<Notification> list)
        {
            list = [];
            context.Items[NotificationsKey] = list;
        }

        Print(notification);

        list.Add(notification);
    }

    /// <summary>
    /// Adds multiple notifications to the current HTTP context and logs them.
    /// </summary>
    /// <param name="notifications">The notifications to add.</param>
    public void Notify(IEnumerable<Notification> notifications)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        if (!context.Items.TryGetValue(NotificationsKey, out var obj) || obj is not List<Notification> list)
        {
            list = [];
            context.Items[NotificationsKey] = list;
        }

        foreach (var notification in notifications)
            Print(notification);

        list.AddRange(notifications);
    }

    /// <summary>
    /// Retrieves the notifications stored in the current HTTP context.
    /// </summary>
    /// <returns>A collection of notifications.</returns>
    public IEnumerable<Notification> Get()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return [];

        if (context.Items.TryGetValue(NotificationsKey, out var obj) && obj is List<Notification> list)
            return list;

        return [];
    }

    /// <summary>
    /// Checks if there are any notifications with a log level higher than Information.
    /// </summary>
    /// <returns><c>true</c> if there are notifications with level greater than Information; otherwise, <c>false</c>.</returns>
    public bool HasNotifications() => Get().Any(x => x.LogLevel > LogLevel.Information);

    /// <summary>
    /// Clears all notifications from the current HTTP context.
    /// </summary>
    public void Clear()
    {
        var context = _httpContextAccessor.HttpContext;
        context?.Items.Remove(NotificationsKey);
    }

    /// <summary>
    /// Logs the notification message according to its log level.
    /// </summary>
    /// <param name="notification">The notification to log.</param>
    private void Print(Notification notification)
    {
        switch (notification.LogLevel)
        {
            case LogLevel.Trace:
                _logger.LogTrace($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.Debug:
                _logger.LogDebug($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.Information:
                _logger.LogInformation($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.Warning:
                _logger.LogWarning($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.Error:
                _logger.LogError($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.Critical:
                _logger.LogCritical($"[Notification] {notification.Key}: {notification.Value}");
                break;
            case LogLevel.None:
                break;
        }
    }
}
