using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;

public interface IAspNetUser
{
    string Name { get; }
    string GetUserCode();
    Guid GetUserId();

    string GetUserEmail();
    string GetUserToken();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim> GetClaims();
    HttpContext GetHttpContext();
}
