using Common.Extensions;
using Common.User.Extensions;
using Microsoft.AspNetCore.Http;

namespace Common.Core.Users;

/// <summary>
/// Represents information about the user making the current HTTP request.
/// </summary>
public class UserRequest
{
    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the client making the request.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the User-Agent header value of the request.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets session-specific request details.
    /// </summary>
    public SessionRequest SessionRequest { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="UserRequest"/> based on the given <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public UserRequest(HttpContext context)
    {
        UserId = context.Request?.GetUserId();
        Name = context.Request?.GetUserName();
        Email = context.User?.GetEmail();
        IpAddress = context.Request?.GetIpAddress();
        UserAgent = context.Request?.GetUserAgent();

        SessionRequest = new SessionRequest
        {
            RequestId = context.Request?.GetRequestId(),
            CorrelationId = context.Request?.GetCorrelationId(),
            ClientId = context.Request?.GetClientId(),
            Method = context.Request?.Method,
            Url = context.Request?.GetFullUrl(),
            PodName = context.Request?.GetPodName(),
            Referer = context.Request?.GetHeader("Referer")
        };
    }
}
