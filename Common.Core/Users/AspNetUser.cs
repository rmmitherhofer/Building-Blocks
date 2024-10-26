using Common.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;
public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AspNetUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public string Name => _httpContextAccessor.HttpContext.GetUserName();

    public string GetUserCode()
        => IsAuthenticated() ? _httpContextAccessor.HttpContext.GetUserCode() : string.Empty;

    public Guid GetUserId()
        => IsAuthenticated() ? Guid.Parse(_httpContextAccessor.HttpContext.GetUserId()) : Guid.Empty;

    public string GetUserEmail()
        => IsAuthenticated() ? _httpContextAccessor.HttpContext.GetUserEmail() : string.Empty;

    public string GetUserToken()
        => IsAuthenticated() ? _httpContextAccessor.HttpContext.GetUserToken() : string.Empty;

    public bool IsAuthenticated()
        => _httpContextAccessor.HttpContext.IsAuthenticated();

    public bool IsInRole(string role)
        => _httpContextAccessor.HttpContext.User.IsInRole(role);

    public IEnumerable<Claim> GetClaims()
        => _httpContextAccessor.HttpContext.User.Claims;

    public HttpContext GetHttpContext()
        => _httpContextAccessor.HttpContext.Validate();
}
