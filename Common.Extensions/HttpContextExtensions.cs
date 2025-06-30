using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;

namespace Common.Extensions;

public static class HttpContextExtensions
{
    public const string USER_ID = "X-User-ID";
    public const string USER_NAME = "X-User-Name";
    public const string USER_LOGIN = "X-User-Login";
    public const string USER_EMAIL = "X-User-Email";
    public const string USER_AGENT = "User-Agent";
    public const string FORWARDED = "X-Forwarded-For";

    public const string REQUEST_ID = "X-Request-ID";
    public const string CORRELATION_ID = "X-Correlation-ID";
    public const string CLIENT_ID = "X-Client-ID";
    public const string POD_NAME = "X-Pod-Name";

    private const string LOCAL_HOST_IP = "127.0.0.1";

    #region User
    public static string? GetUserId(this HttpContext context)
        => context.User.GetId() ?? context.Request?.GetHeader(USER_ID) ?? string.Empty;
    public static string? GetUserName(this HttpContext context) => context.User.GetName();
    public static string? GetUserLogin(this HttpContext context)
        => context.User.GetClaim(USER_LOGIN);
    public static string? GetUserEmail(this HttpContext context)
        => context.User.GetClaim(USER_EMAIL);
    public static string GetIpAddress(this HttpContext context)
    {
        string? ipAddress = context.User.GetClaim(FORWARDED) ?? context.Request?.GetHeader(FORWARDED) ?? string.Empty;

        if (Debugger.IsAttached || string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection?.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Equals("::1"))
            {
                ipAddress = LOCAL_HOST_IP;
                context.Request.AddHeader(FORWARDED, LOCAL_HOST_IP);
            }
        }
        return ipAddress.Split(',')[0].Replace("{", "").Replace("}", "");
    }
    public static string? GetUserAgent(this HttpContext context)
        => context.User.GetClaim(USER_AGENT) ?? context.Request?.GetHeader(USER_AGENT);
    public static bool IsAuthenticated(this HttpContext context) => context.User.IsAuthenticated();
    public static void AddUserId(this HttpContext context, string userId)
        => AddClaim(context, USER_ID, userId);
    public static void AddUserName(this HttpContext context, string userName)
        => AddClaim(context, USER_NAME, userName);
    public static void AddUserLogin(this HttpContext context, string userLogin)
        => AddClaim(context, USER_LOGIN, userLogin);
    public static void AddUserEmail(this HttpContext context, string userEmail)
        => AddClaim(context, USER_EMAIL, userEmail);
    public static void AddUserAgent(this HttpContext context, string userAgent)
        => AddClaim(context, USER_AGENT, userAgent);
    public static void AddIpAddress(this HttpContext context, string ipAddress)
        => AddClaim(context, FORWARDED, ipAddress);
    #endregion

    #region Request
    public static string GetRequestId(this HttpContext context)
    {
        context.CreateRequestId();

        return context.User.GetClaim(REQUEST_ID) ?? context.Request?.GetHeader(REQUEST_ID);
    }

    public static string? GetCorrelationId(this HttpContext context)
    {
        context.CreateCorrelationId();

        return context.User.GetClaim(CORRELATION_ID) ?? context.Request?.GetHeader(CORRELATION_ID);
    }

    public static string? GetClientId(this HttpContext context)
        => context.User.GetClaim(CLIENT_ID) ?? context.Request?.GetHeader(CLIENT_ID);
    public static string? GetPodName(this HttpContext context)
        => context.User.GetClaim(POD_NAME) ?? context.Request?.GetHeader(POD_NAME);

    public static void CreateRequestId(this HttpContext context)
    {
        if (!string.IsNullOrEmpty(context.User.GetClaim(REQUEST_ID) ?? context.Request?.GetHeader(REQUEST_ID))) return;

        context.Request.AddHeader(REQUEST_ID, $"{Guid.NewGuid().ToString()[..8]}-{DateTime.Now:ddMMyyyy-HHmmss}");
    }
    public static void CreateCorrelationId(this HttpContext context)
    {
        if (!string.IsNullOrEmpty(context.User.GetClaim(CORRELATION_ID) ?? context.Request?.GetHeader(CORRELATION_ID))) return;

        context.Request.AddHeader(CORRELATION_ID, context.GetRequestId());
    }
    #endregion

    #region Common
    private static void AddClaim(HttpContext context, string key, string value)
    {
        context.Validate();

        context.User.AddClaim(key, value);
    }

    private static string? FindClaim(this HttpContext context, string type)
    {
        context.Validate();

        return context.User.GetClaim(type);
    }
    public static HttpContext Validate(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(HttpContext));

        return context;
    }
    #endregion
}
