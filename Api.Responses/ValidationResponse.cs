using Common.Notifications.Messages;
using System.Text.Json.Serialization;

namespace Api.Responses;

/// <summary>
/// Represents a response that contains validation error notifications.
/// </summary>
public class ValidationResponse
{
    /// <summary>
    /// Gets the list of validation notifications returned by the API.
    /// </summary>
    [JsonPropertyName("validations")]
    public IEnumerable<Notification> Validations { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResponse"/> class with the specified notifications.
    /// </summary>
    /// <param name="notifications">The collection of validation notifications.</param>
    public ValidationResponse(IEnumerable<Notification> notifications)
        => Validations = notifications;
}
