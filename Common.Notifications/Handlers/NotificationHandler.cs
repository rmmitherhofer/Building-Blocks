using Common.Notifications.Interfaces;
using Common.Notifications.Messages;

namespace Common.Notifications.Handlers;

public class NotificationHandler : INotificationHandler
{
    private readonly List<Notification> _notificacoes;

    public NotificationHandler() => _notificacoes = [];

    /// <summary>
    /// Armazena notificação no contexto
    /// </summary>
    /// <param name="notificacao"></param>
    public void Notify(Notification notificacao) => _notificacoes.Add(notificacao);

    /// <summary>
    /// Armazena lista de notificações no contexto
    /// </summary>
    /// <param name="notificacoes"></param>
    public void Notificar(ICollection<Notification> notificacoes) => _notificacoes.AddRange(notificacoes);

    /// <summary>
    /// Retorna lista de notificações
    /// </summary>
    /// <returns>Coleção de Notificações</returns>
    public ICollection<Notification> Get() => _notificacoes;

    /// <summary>
    /// Valida se possui notificações no contexto
    /// </summary>
    /// <returns>true or false</returns>
    public bool HasNotifications() => _notificacoes.Count > 0;

    /// <summary>
    /// Limpa a lista de notificações no contexto
    /// </summary>
    public void Clear() => _notificacoes.Clear();
}
