using Common.Enums;
using Common.Notifications.Messages;

namespace Api.Responses;

/// <summary>
/// Represents a detailed response segment issued by the API for a specific category of result, such as errors or validation issues.
/// </summary>
public class IssuerResponse
{
    /// <summary>
    /// Gets the type of issue returned by the API (e.g., validation, error, not found).
    /// </summary>
    public IssuerResponseType Type { get; private set; }

    /// <summary>
    /// Gets the description of the issue type, usually derived from an attribute or enum name.
    /// </summary>
    public string DescriptionType { get; private set; }

    /// <summary>
    /// Gets or sets the title that summarizes the issue.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed list of notifications associated with the issue.
    /// </summary>
    public IEnumerable<Notification>? Details { get; set; }

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
