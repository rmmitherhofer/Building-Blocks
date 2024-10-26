using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;

namespace Common.Extensions;

public static class HttpContextExtensions
{
    public const string USER_ID = "user-id";
    public const string USER_CODE = "user-code";
    public const string USER_IP_ADDRESS = "user-ip-address";
    public const string USER_SESSION_ID = "user-session-id";
    public const string USER_TOKEN = "user-token";
    public const string USER_EMAIL= "user-email";
    public const string FORWARDED = "x-forwarded-for";
    public const string LOCAL_HOST_IP = "127.0.0.1";

    public static string GetUserCode(this HttpContext context) 
        => context.Find(USER_CODE)?.Value ?? string.Empty;

    public static string GetUserId(this HttpContext context) 
        => context.Find(USER_ID)?.Value ?? string.Empty;

    public static string GetSessionId(this HttpContext context) 
        => context.Find(USER_SESSION_ID)?.Value ?? string.Empty;

    public static string GetIpAddress(this HttpContext context) 
        => context.Find(USER_IP_ADDRESS)?.Value ?? string.Empty;

    public static string GetIpAddressForwarded(this HttpContext context)
    {
        context.Validate();

        string? ipAddress = context.Request?.Headers?.FirstOrDefault(h => h.Key == FORWARDED).Value;

        if(Debugger.IsAttached || string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection?.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Equals("::1"))
                ipAddress = LOCAL_HOST_IP;
        }

        return ipAddress.Split(',')[0].Replace("{","").Replace("}", "");
    }

    public static string GetUserName(this HttpContext context)
    {
        context.ClaimsValidate();

        return context.User.Identity.Name ?? string.Empty;
    }

    public static bool IsAuthenticated(this HttpContext context)
    {
        context.ClaimsValidate();

        return context.User.Identity.IsAuthenticated;
    }

    public static string GetUserToken(this HttpContext context) 
        => context.Find(USER_TOKEN)?.Value ?? string.Empty;

    public static string GetUserEmail(this HttpContext context) 
        => context.Find(USER_EMAIL)?.Value ?? string.Empty;

    public static void AddClaimSessionId(this HttpContext context)
        => AddClaim(context, USER_SESSION_ID, $"{Guid.NewGuid().ToString()[..8]}-{DateTime.Now:ddMMyyyy-HHmmss}");

    public static void AddClaimSessionId(this HttpContext context, string sessionId)
        => AddClaim(context, USER_SESSION_ID, sessionId);

    public static void AddClaimIpAddress(this HttpContext context, string ipAddress) 
        => AddClaim(context, USER_IP_ADDRESS, ipAddress);

    public static void AddClaimUserId(this HttpContext context, string userId)
        => AddClaim(context, USER_ID, userId);

    public static void AddClaimUserCode(this HttpContext context, string userCode)
        => AddClaim(context, USER_CODE, userCode);

    private static void AddClaim(HttpContext context, string type, string value)
    {
        context.ClaimsValidate();

        if (string.IsNullOrEmpty(value)) return;

        var claimsPrincipal = context.User;

        foreach (var identity in claimsPrincipal.Identities)
            identity.AddClaim(new Claim(type, value));
    }

    private static Claim Find(this HttpContext context, string type)
    {
        context.ClaimsValidate();

        return context.User.FindFirst(type);
    }


    public static HttpContext Validate(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(HttpContext));

        return context;
    }
    private static ClaimsPrincipal ClaimsValidate(this HttpContext context)
    {
        context.Validate();

        ArgumentNullException.ThrowIfNull(context.User, nameof(ClaimsPrincipal));     
        
        return context.User;
    }
}
