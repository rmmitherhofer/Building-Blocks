using Common.Notifications.Messages;
using System.Text.Json.Serialization;

namespace Api.Responses;

/// <summary>
/// Classe de resposta de validações
/// </summary>
public class ValidationResponse
{
    /// <summary>
    /// Notificações
    /// </summary>
    [JsonPropertyName("validations")]
    public IEnumerable<Notification> Validations { get; private set; }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="messages"></param>
    public ValidationResponse(IEnumerable<Notification> notifications)
    {
        Validations = notifications;
    }
}