using System.Security.Claims;

namespace Common.User.Extensions;

/// <summary>
/// Extension methods to simplify working with ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Claim key for tenant identifier.</summary>
    public const string TENANT_ID = "tenantId";
    /// <summary>Claim key for company identifier.</summary>
    public const string COMPANY_ID = "companyId";
    /// <summary>Claim key for department.</summary>
    public const string DEPARTMENT = "department";
    /// <summary>Claim key for user profile.</summary>
    public const string PROFILE = "profile";
    /// <summary>Claim key for locale.</summary>
    public const string LOCALE = "locale";
    /// <summary>Claim key for session identifier.</summary>
    public const string SESSION_ID = "sessionId";
    /// <summary>Claim key for user identifier.</summary>
    public const string USER_ID = "userId";
    /// <summary>Claim key for user account.</summary>
    public const string USER_ACCOUNT= "userAccount";
    /// <summary>Claim key for user account code.</summary>
    public const string USER_ACCOUNT_CODE = "userAccountCode";
    /// <summary>Claim key for user document.</summary>
    public const string USER_DOCUMENT = "userDocument";
    /// <summary>Claim key for expiration timestamp.</summary>
    public const string EXPIRATION = "exp";
    /// <summary>Claim key for language.</summary>
    public const string LANGUAGE = "lang";
    /// <summary>Claim key for subject identifier.</summary>
    public const string SUB = "sub";

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>True if authenticated; otherwise false.</returns>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.Identity?.IsAuthenticated ?? false;
    }
    /// <summary>
    /// Gets the value of a claim by key.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    /// <returns>Claim value or null if not found.</returns>
    public static string? GetClaim(this ClaimsPrincipal user, string key)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.FindFirst(key)?.Value;
    }
    /// <summary>
    /// Adds a claim to the user if it does not already exist.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    /// <param name="value">Claim value.</param>
    public static void AddClaim(this ClaimsPrincipal user, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        foreach (var identity in user.Identities)
        {
            if (!identity.HasClaim(c => c.Type == key && c.Value == value))
                identity.AddClaim(new Claim(key, value));
        }
    }
    /// <summary>
    /// Adds a claim or updates its value if it already exists.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    /// <param name="value">Claim value.</param>
    public static void AddOrUpdateClaim(this ClaimsPrincipal user, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        foreach (var identity in user.Identities)
        {
            foreach (var claim in identity.FindAll(key).ToList())
                identity.TryRemoveClaim(claim);

            identity.AddClaim(new Claim(key, value));
        }
    }
    /// <summary>
    /// Removes all claims matching the specified key.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="key">Claim type/key.</param>
    public static void RemoveClaim(this ClaimsPrincipal user, string key)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        foreach (var identity in user.Identities)
        {
            foreach (var claim in identity.FindAll(key).ToList())
                identity.TryRemoveClaim(claim);
        }
    }
    /// <summary>
    /// Gets the user's identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>User Id claim value or subject claim if user Id is absent.</returns>
    public static string? GetId(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.NameIdentifier) ?? user.GetClaim(SUB);
    /// <summary>
    /// Adds a user identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">User Id value.</param>
    public static void AddId(this ClaimsPrincipal user, string value) => user.AddClaim(ClaimTypes.NameIdentifier, value);
    /// <summary>
    /// Gets the user's name claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Name claim value or the Identity's Name if claim is missing.</returns>
    public static string? GetName(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.Name) ?? user.Identity?.Name;
    /// <summary>
    /// Adds a name claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Name value.</param>
    public static void AddName(this ClaimsPrincipal user, string value) => user.AddClaim(ClaimTypes.Name, value);
    /// <summary>
    /// Gets the user's email claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Email claim value or null.</returns>
    public static string? GetEmail(this ClaimsPrincipal user) => user.GetClaim(ClaimTypes.Email);
    /// <summary>
    /// Adds an email claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Email value.</param>
    public static void AddEmail(this ClaimsPrincipal user, string value) => user.AddClaim(ClaimTypes.Email, value);
    /// <summary>
    /// Gets the list of role claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>IEnumerable of role claim values.</returns>
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
    /// <summary>
    /// Adds a role claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Role value.</param>
    public static void AddRole(this ClaimsPrincipal user, string value) => user.AddClaim(ClaimTypes.Role, value);
    /// <summary>
    /// Gets the authentication type.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Authentication type or null.</returns>
    public static string? GetAuthenticationType(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(ClaimsPrincipal));
        return user?.Identity?.AuthenticationType;
    }
    /// <summary>
    /// Gets the tenant identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Tenant Id claim value or null.</returns>
    public static string? GetTenantId(this ClaimsPrincipal user) => user.GetClaim(TENANT_ID);
    /// <summary>
    /// Adds a tenant identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Tenant Id value.</param>
    public static void AddTenantId(this ClaimsPrincipal user, string value) => user.AddClaim(TENANT_ID, value);
    /// <summary>
    /// Gets the company identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Company Id claim value or null.</returns>
    public static string? GetCompanyId(this ClaimsPrincipal user) => user.GetClaim(COMPANY_ID);
    /// <summary>
    /// Adds a company identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Company Id value.</param>
    public static void AddCompanyId(this ClaimsPrincipal user, string value) => user.AddClaim(COMPANY_ID, value);
    /// <summary>
    /// Gets the department claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Department claim value or null.</returns>
    public static string? GetDepartment(this ClaimsPrincipal user) => user.GetClaim(DEPARTMENT);
    /// <summary>
    /// Adds a department claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Department value.</param>
    public static void AddDepartment(this ClaimsPrincipal user, string value) => user.AddClaim(DEPARTMENT, value);
    /// <summary>
    /// Gets the profile claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Profile claim value or null.</returns>
    public static string? GetProfile(this ClaimsPrincipal user) => user.GetClaim(PROFILE);
    /// <summary>
    /// Adds a profile claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Profile value.</param>
    public static void AddProfile(this ClaimsPrincipal user, string value) => user.AddClaim(PROFILE, value);
    /// <summary>
    /// Gets the locale or language claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Locale or language claim value or null.</returns>
    public static string? GetLocale(this ClaimsPrincipal user) => user.GetClaim(LOCALE) ?? user.GetClaim(LANGUAGE);
    /// <summary>
    /// Adds a locale claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Locale value.</param>
    public static void AddLocale(this ClaimsPrincipal user, string value) => user.AddClaim(LOCALE, value);
    /// <summary>
    /// Gets the expiration date/time claim as a UTC DateTime.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Expiration DateTime or null if not set or invalid.</returns>
    public static DateTime? GetExpiration(this ClaimsPrincipal user)
    {
        var expClaim = user.GetClaim(EXPIRATION);
        if (long.TryParse(expClaim, out var seconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }
        return null;
    }
    /// <summary>
    /// Adds an expiration claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Expiration value as string.</param>
    public static void AddExpiration(this ClaimsPrincipal user, string value) => user.AddClaim(EXPIRATION, value);
    /// <summary>
    /// Gets the session identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetSessionId(this ClaimsPrincipal user) => user.GetClaim(SESSION_ID);
    /// <summary>
    /// Adds a session identifier claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Session Id value.</param>
    public static void AddSessionId(this ClaimsPrincipal user, string value) => user.AddClaim(SESSION_ID, value);
    /// <summary>
    /// Gets the user document claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetDocument(this ClaimsPrincipal user) => user.GetClaim(USER_DOCUMENT);
    /// <summary>
    /// Adds a user document claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Session Id value.</param>
    public static void AddDocument(this ClaimsPrincipal user, string value) => user.AddClaim(USER_DOCUMENT, value);
    /// <summary>
    /// Gets the user account claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetAccount(this ClaimsPrincipal user) => user.GetClaim(USER_ACCOUNT);
    /// <summary>
    /// Adds a user account claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Session Id value.</param>
    public static void AddAccount(this ClaimsPrincipal user, string value) => user.AddClaim(USER_ACCOUNT, value);
    /// <summary>
    /// Gets the user account code claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <returns>Session Id claim value or null.</returns>
    public static string? GetAccountCode(this ClaimsPrincipal user) => user.GetClaim(USER_ACCOUNT_CODE);
    /// <summary>
    /// Adds a user account code claim.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal instance.</param>
    /// <param name="value">Session Id value.</param>
    public static void AddAccountCode(this ClaimsPrincipal user, string value) => user.AddClaim(USER_ACCOUNT_CODE, value);
}
