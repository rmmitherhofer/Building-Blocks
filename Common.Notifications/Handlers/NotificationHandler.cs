using Common.Notifications.Interfaces;
using Common.Notifications.Messages;

namespace Common.Notifications.Handlers;

public class NotificationHandler : INotificationHandler
{
    private readonly List<Notification> _notifications;

    public NotificationHandler() => _notifications = [];

    /// <summary>
    /// Armazena notificação no contexto
    /// </summary>
    /// <param name="notification"></param>
    public void Notify(Notification notification) => _notifications.Add(notification);

    /// <summary>
    /// Armazena lista de notificações no contexto
    /// </summary>
    /// <param name="notifications"></param>
    public void Notificar(ICollection<Notification> notifications) => _notifications.AddRange(notifications);

    /// <summary>
    /// Retorna lista de notificações
    /// </summary>
    /// <returns>Coleção de Notificações</returns>
    public ICollection<Notification> Get() => _notifications;

    /// <summary>
    /// Valida se possui notificações no contexto
    /// </summary>
    /// <returns>true or false</returns>
    public bool HasNotifications() => _notifications.Count > 0;

    /// <summary>
    /// Limpa a lista de notificações no contexto
    /// </summary>
    public void Clear() => _notifications.Clear();
}
