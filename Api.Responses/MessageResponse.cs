namespace Api.Responses;

/// <summary>
/// Represents the base class for structured API messages that belong to a specific aggregate context.
/// </summary>
public abstract class MessageResponse
{
    /// <summary>
    /// Gets or sets the type of the message, typically used to classify the event or action.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the aggregate root identifier related to the message context.
    /// </summary>
    public Guid AggregateId { get; set; }
}
