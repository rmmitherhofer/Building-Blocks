using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Common.Extensions;

public static class HttpRequestExtensions
{
    public const string CORRELATION_ID = "X-Correlation-ID";
    public static void SetCorrelationId(this HttpRequest response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpRequest));

        if (response.Headers.ContainsKey(CORRELATION_ID))
            response.Headers.Remove(CORRELATION_ID);

        response.Headers.TryAdd(CORRELATION_ID, response.HttpContext.GetCorrelationId() ?? response.HttpContext.GetRequestId());
    }

    public static void AddHeader(this HttpRequest response, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpRequest));
        ArgumentNullException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNullOrEmpty(value, nameof(value));

        if (response.Headers.ContainsKey(key))
            response.Headers.Remove(key);

        response.Headers.TryAdd(key, value);
    }

    public static string GetHeader(this HttpRequest request, string key)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequest));

        ArgumentNullException.ThrowIfNullOrEmpty(key, nameof(key));

        if (request.Headers.TryGetValue(key, out var value))
            return value.ToString();

        return null;
    }
}

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
        ArgumentNullException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentNullException.ThrowIfNullOrEmpty(value, nameof(value));

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