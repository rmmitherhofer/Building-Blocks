using System.Security.Claims;

namespace Common.Core.Users;

/// <summary>
/// Interface to provide information about the current ASP.NET user.
/// </summary>
public interface IAspNetUser
{
    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the current user's identifier.
    /// </summary>
    /// <returns>User ID as string.</returns>
    string GetUserId();

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    /// <returns>User email as string.</returns>
    string GetUserEmail();

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if authenticated; otherwise, false.</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Checks if the current user is in a specific role.
    /// </summary>
    /// <param name="role">Role name to check.</param>
    /// <returns>True if user is in the role; otherwise, false.</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Gets the claims associated with the current user.
    /// </summary>
    /// <returns>An enumerable collection of claims.</returns>
    IEnumerable<Claim> GetClaims();
}
