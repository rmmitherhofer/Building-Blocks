using Common.User.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Core.Users;

/// <summary>
/// Implementation of <see cref="IAspNetUser"/> that retrieves user information from the current HTTP context.
/// </summary>
public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _accessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetUser"/> class.
    /// </summary>
    /// <param name="accessor">HTTP context accessor to access the current user.</param>
    public AspNetUser(IHttpContextAccessor accessor) => _accessor = accessor;

    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    public string? Name => _accessor.HttpContext.User.GetName();

    /// <summary>
    /// Gets the current user's identifier.
    /// </summary>
    /// <returns>User ID as string or null if not available.</returns>
    public string? Id => _accessor.HttpContext.User.GetId();

    /// <summary>
    /// Gets the current user's account code.
    /// </summary>
    /// <returns>User Account as string or null if not available.</returns>
    public string? AccountCode => _accessor.HttpContext.User.GetAccountCode();

    /// <summary>
    /// Gets the current user's account.
    /// </summary>
    /// <returns>User Account as string or null if not available.</returns>
    public string? Account => _accessor.HttpContext.User.GetAccount();

    /// <summary>
    /// Gets the current user's document.
    /// </summary>
    /// <returns>User Document as string or null if not available.</returns>
    public string? Document => _accessor.HttpContext.User.GetDocument();

    /// <summary>
    /// Gets the current user's departament.
    /// </summary>
    /// <returns>User departament as string or null if not available.</returns>
    public string? Departament => _accessor.HttpContext.User.GetDepartment();

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    /// <returns>User email as string or null if not available.</returns>
    public string? Email => _accessor.HttpContext.User.GetEmail();

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if authenticated; otherwise, false.</returns>
    public bool IsAuthenticated => _accessor.HttpContext.User.IsAuthenticated();

    /// <summary>
    /// Checks if the current user is in the specified role.
    /// </summary>
    /// <param name="role">Role name to check.</param>
    /// <returns>True if the user is in the role; otherwise, false.</returns>
    public bool IsInRole(string role) => _accessor.HttpContext.User.IsInRole(role);

    /// <summary>
    /// Gets the claims of the current user.
    /// </summary>
    /// <returns>An enumerable of <see cref="Claim"/> objects.</returns>
    public IEnumerable<Claim> GetClaims() => _accessor.HttpContext.User.Claims;
}
