using System.Security.Claims;

namespace Zypher.Http.Extensions;

/// <summary>
/// Extension methods to simplify working with ClaimsPrincipal.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>Claim key for subject identifier.</summary>
    internal const string SUB = "sub";
    /// <summary>
    /// Gets the value of a claim by key.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    /// <returns>Claim value or null if not found.</returns>
    internal static string? GetClaim(this ClaimsPrincipal user, string key)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.FindFirst(key)?.Value;
    }

    /// <summary>
    /// Gets the user's identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>User Id claim value or subject claim if user Id is absent.</returns>
    internal static string? GetId(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.NameIdentifier) ?? user.GetClaim(SUB);
 
}
