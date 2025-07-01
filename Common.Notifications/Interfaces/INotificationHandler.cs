using Common.Notifications.Messages;

namespace Common.Notifications.Interfaces;

/// <summary>
/// Handler para orquestrar notificações no contexto
/// </summary>
public interface INotificationHandler
{
    /// <summary>
    /// Armazena notificação no contexto
    /// </summary>
    /// <param name="notification"></param>
    void Notify(Notification notification);

    /// <summary>
    /// Armazena lista de notificações no contexto
    /// </summary>
    /// <param name="notifications"></param>
    void Notify(IEnumerable<Notification> notifications);

    /// <summary>
    /// Retorna lista de notificações
    /// </summary>
    /// <returns>Coleção de Notificações</returns>
    IEnumerable<Notification> Get();

    /// <summary>
    /// Valida se possui notificações no contexto
    /// </summary>
    /// <returns>true or false</returns>
    bool HasNotifications();

    /// <summary>
    /// Limpa a lista de notificações no contexto
    /// </summary>
    void Clear();
}