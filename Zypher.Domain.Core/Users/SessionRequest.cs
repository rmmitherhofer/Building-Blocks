namespace Zypher.Domain.Core.Users;

/// <summary>
/// Represents information about an HTTP session request.
/// </summary>
public class SessionRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for the request.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for tracking requests.
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method used in the request (e.g., GET, POST).
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Gets or sets the URL of the request.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the referer URL from which the request originated.
    /// </summary>
    public string? Referer { get; set; }

    /// <summary>
    /// Gets or sets the name of the pod (server or container) handling the request.
    /// </summary>
    public string? PodName { get; set; }

}
