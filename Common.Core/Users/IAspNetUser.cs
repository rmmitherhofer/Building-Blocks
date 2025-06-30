using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;

public interface IAspNetUser
{
    string Name { get; }
    string GetUserId();
    string GetUserEmail();
    string GetUserLogin();
    string GetIpAddress();
    string GetUserAgent();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim> GetClaims();
    HttpContext GetHttpContext();
}