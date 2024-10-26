namespace Common.Notifications.Messages;

public abstract class Message
{
    /// <summary>
    /// Tipo de mensagem
    /// </summary>
    public string Type { get; protected set; }

    /// <summary>
    /// Id de Agregacao
    /// </summary>
    public Guid AgregationId { get; protected set; }

    /// <summary>
    /// Construtor
    /// </summary>
    protected Message() => Type = GetType().Name;

    protected Message(string type) => Type = type;
}

