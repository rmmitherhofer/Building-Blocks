namespace Common.Notifications.Messages;

public abstract class Message
{
    /// <summary>
    /// Tipo de mensagem
    /// </summary>
    public string Tipo { get; protected set; }

    /// <summary>
    /// Id de Agregacao
    /// </summary>
    public Guid IdAgregacao { get; protected set; }

    /// <summary>
    /// Construtor
    /// </summary>
    protected Message() => Tipo = GetType().Name;

    protected Message(string tipo) => Tipo = tipo;
}

