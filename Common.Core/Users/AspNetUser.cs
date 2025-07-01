using Common.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;
public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _accessor;
    public AspNetUser(IHttpContextAccessor accessor) => _accessor = accessor;
    public string? Name => _accessor.HttpContext.GetUserName();
    public string? GetUserId() => _accessor.HttpContext.GetUserId();
    public string? GetUserEmail() => _accessor.HttpContext.GetUserEmail();
    public string? GetUserLogin() => _accessor.HttpContext.GetUserLogin();
    public string? GetIpAddress() => _accessor.HttpContext.GetIpAddress();
    public string? GetUserAgent() => _accessor.HttpContext.GetUserAgent();
    public bool IsAuthenticated() => _accessor.HttpContext.IsAuthenticated();
    public bool IsInRole(string role) => _accessor.HttpContext.User.IsInRole(role);
    public IEnumerable<Claim> GetClaims() => _accessor.HttpContext.User.Claims;
    public HttpContext GetHttpContext() => _accessor.HttpContext.Validate();
}
