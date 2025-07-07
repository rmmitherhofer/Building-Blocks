namespace Common.Notifications.Messages;

public abstract class Message
{
    /// <summary>
    /// Type of the message.
    /// </summary>
    public string Type { get; protected set; }

    /// <summary>
    /// Aggregation Id.
    /// </summary>
    public Guid AgregationId { get; protected set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected Message() => Type = GetType().Name;

    /// <summary>
    /// Constructor with explicit type.
    /// </summary>
    /// <param name="type">The message type.</param>
    protected Message(string type) => Type = type;
}
