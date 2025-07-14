using System.Text.Json.Serialization;
using Zypher.Enums;

namespace Zypher.Responses;

/// <summary>
/// Represents a detailed response segment issued by the API for a specific category of result, such as errors or validation issues.
/// </summary>
public class IssuerResponse
{
    /// <summary>
    /// Gets the type of issue returned by the API (e.g., validation, error, not found).
    /// </summary>

    [JsonPropertyName("type")]
    public IssuerResponseType Type { get; set; }

    /// <summary>
    /// Gets the description of the issue type, usually derived from an attribute or enum name.
    /// </summary>
    [JsonPropertyName("descriptionType")]
    public string DescriptionType { get; set; }

    /// <summary>
    /// Gets or sets the title that summarizes the issue.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed list of notifications associated with the issue.
    /// </summary>
    [JsonPropertyName("details")]
    public IEnumerable<NotificationResponse>? Details { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="IssuerResponse"/> class.
    /// </summary>
    public IssuerResponse() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="IssuerResponse"/> class using a specific issue type.
    /// </summary>
    /// <param name="type">The type of the issue (e.g., Error, Validation, NotFound).</param>
    public IssuerResponse(IssuerResponseType type)
    {
        Type = type;
        DescriptionType = type.GetDescription();
    }
}
