using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Notifications.Handlers;

public class NotificationHandler : INotificationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<NotificationHandler> _logger;
    private const string NotificationsKey = "__Notifications__";

    public NotificationHandler(IHttpContextAccessor httpContextAccessor, ILogger<NotificationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Valida se possui notificações no contexto
    /// </summary>
    /// <returns>true or false</returns>
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
    /// Armazena lista de notificações no contexto
    /// </summary>
    /// <param name="notifications"></param>
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
    /// Retorna lista de notificações
    /// </summary>
    /// <returns>Coleção de Notificações</returns>
    public IEnumerable<Notification> Get()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return Enumerable.Empty<Notification>();

        if (context.Items.TryGetValue(NotificationsKey, out var obj) && obj is List<Notification> list)
            return list;

        return [];
    }

    /// <summary>
    /// Valida se possui notificações no contexto
    /// </summary>
    /// <returns>true or false</returns>
    public bool HasNotifications() => Get().Any(x => x.LogLevel > LogLevel.Information);

    /// <summary>
    /// Limpa a lista de notificações no contexto
    /// </summary>
    public void Clear()
    {
        var context = _httpContextAccessor.HttpContext;
        context?.Items.Remove(NotificationsKey);
    }

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
