using System.Security.Claims;

namespace Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public const string USER_ID = "X-User-ID";
    public const string USER_NAME = "X-User-Name";
    public const string USER_LOGIN = "X-User-Login";
    public const string USER_EMAIL = "X-User-Email";
    public const string USER_AGENT = "User-Agent";
    public const string FORWARDED = "X-Forwarded-For";

    public static void Validate(this ClaimsPrincipal user) => ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
    public static string? GetClaim(this ClaimsPrincipal user, string key)
    {
        user.Validate();

        return user.FindFirst(key)?.Value;
    }
    public static void AddClaim(this ClaimsPrincipal user, string key, string value)
    {
        user.Validate();
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        foreach (var identity in user.Identities)
        {
            if (!identity.HasClaim(c => c.Type == key && c.Value == value))
                identity.AddClaim(new Claim(key, value));
        }
    }

    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        user.Validate();

        return user.Identity?.IsAuthenticated ?? false;
    }
    public static string GetName(this ClaimsPrincipal user)
    {
        user.Validate();

        return user.Identity.Name ?? user.GetClaim(USER_NAME);
    }
    public static string GetId(this ClaimsPrincipal user)
    {
        user.Validate();

        return user.GetClaim(USER_ID);
    }
}