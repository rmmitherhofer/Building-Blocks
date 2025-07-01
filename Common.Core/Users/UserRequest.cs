using Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace Common.Core.Users;

public class UserRequest
{
    public string UserId { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public SessionRequest SessionRequest { get; set; }

    public UserRequest(HttpContext context)
    {
        UserId = context.GetUserId();
        Login = context.GetUserLogin();
        Name = context.GetUserName();
        Email = context.GetUserEmail();
        IpAddress = context.GetIpAddress();
        UserAgent = context.GetUserAgent();

        SessionRequest = new SessionRequest
        {
            RequestId = context.GetRequestId(),
            CorrelationId = context.GetCorrelationId(),
            ClientId = context.GetClientId(),
            Method = context.Request.Method,
            Url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}",
            Referer = context.Request.GetHeader("Referer")
        };
    }
}
