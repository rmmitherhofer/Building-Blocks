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
    string Id { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    /// <returns>User email as string.</returns>
    string Email { get; }

    /// <summary>
    /// Gets the current user's account.
    /// </summary>
    /// <returns>User Account code as string or null if not available.</returns>
    string? AccountCode { get; }
    /// <summary>
    /// Gets the current user's account.
    /// </summary>
    /// <returns>User Account as string or null if not available.</returns>
    string? Account { get; }

    /// <summary>
    /// Gets the current user's document.
    /// </summary>
    /// <returns>User Document as string or null if not available.</returns>
    string? Document { get; }

    /// <summary>
    /// Gets the current user's departament.
    /// </summary>
    /// <returns>User departament as string or null if not available.</returns>
    string? Departament { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if authenticated; otherwise, false.</returns>
    bool IsAuthenticated { get; }

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
