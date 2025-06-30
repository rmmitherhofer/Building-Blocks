using Common.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;
public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AspNetUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;
    public string? Name => _httpContextAccessor.HttpContext.GetUserName();
    public string? GetUserId() => _httpContextAccessor.HttpContext.GetUserId();
    public string? GetUserEmail() => _httpContextAccessor.HttpContext.GetUserEmail();
    public string? GetUserLogin() => _httpContextAccessor.HttpContext.GetUserLogin();
    public string? GetIpAddress() => _httpContextAccessor.HttpContext.GetIpAddress();
    public string? GetUserAgent() => _httpContextAccessor.HttpContext.GetUserAgent();
    public bool IsAuthenticated() => _httpContextAccessor.HttpContext.IsAuthenticated();
    public bool IsInRole(string role) => _httpContextAccessor.HttpContext.User.IsInRole(role);
    public IEnumerable<Claim> GetClaims() => _httpContextAccessor.HttpContext.User.Claims;
    public HttpContext GetHttpContext() => _httpContextAccessor.HttpContext.Validate();
}
