using System.Text.Json.Serialization;

namespace Zypher.Responses;

/// <summary>
/// Represents a response that contains validation error notifications.
/// </summary>
public class ValidationResponse
{
    /// <summary>
    /// Gets the list of validation notifications returned by the API.
    /// </summary>
    [JsonPropertyName("validations")]
    public List<NotificationResponse> Validations { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResponse"/> class.
    /// </summary>
    public ValidationResponse() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResponse"/> class with the specified notifications.
    /// </summary>
    /// <param name="notifications">The collection of validation notifications.</param>
    public ValidationResponse(IEnumerable<NotificationResponse> notifications)
        => Validations = [.. notifications];
}
