using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;

namespace Zypher.Http.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequest"/> to simplify common HTTP request operations.
/// </summary>
internal static class HttpRequestExtensions
{
    /// <summary>X-User-ID header key.</summary>
    internal const string USER_ID = "X-User-ID";
    /// <summary>X-User-Name header key.</summary>
    internal const string USER_NAME = "X-User-Name";
    /// <summary>X-User-Name header key.</summary>
    internal const string USER_ACCOUNT = "X-User-Account";
    /// <summary>X-User-Name-Code header key.</summary>
    internal const string USER_ACCOUNT_CODE = "X-User-Account-Code";
    /// <summary>X-User-Name header key.</summary>
    internal const string USER_DOCUMENT = "X-User-Document";
    /// <summary>User-Agent header key.</summary>
    internal const string USER_AGENT = "User-Agent";
    /// <summary>X-Forwarded-For header key.</summary>
    internal const string FORWARDED = "X-Forwarded-For";
    /// <summary>X-Requested-With header key.</summary>
    internal const string REQUESTED_WITH = "X-Requested-With";

    /// <summary>X-Request-ID header key.</summary>
    internal const string REQUEST_ID = "X-Request-ID";
    /// <summary>X-Correlation-ID header key.</summary>
    internal const string CORRELATION_ID = "X-Correlation-ID";
    /// <summary>X-Client-ID header key.</summary>
    internal const string CLIENT_ID = "X-Client-ID";
    /// <summary>X-Pod-Name header key.</summary>
    internal const string POD_NAME = "X-Pod-Name";

    private const string LOCAL_HOST_IP = "127.0.0.1";

    /// <summary>
    /// Gets the user ID from the request header.
    /// </summary>
    internal static string? GetUserId(this HttpRequest request) => request.GetHeader(USER_ID);

    /// <summary>
    /// Gets the user account from the request header.
    /// </summary>
    internal static string? GetUserAccount(this HttpRequest request) => request.GetHeader(USER_ACCOUNT);
    /// <summary>
    /// Gets the user agent string from the request header.
    /// </summary>
    internal static string? GetUserAgent(this HttpRequest request) => request.GetHeader(USER_AGENT);

    /// <summary>
    /// Gets the IP address of the client making the request, considering forwarded headers.
    /// Defaults to localhost if no IP found.
    /// </summary>
    internal static string GetIpAddress(this HttpRequest request)
    {
        string ipAddress = request.GetHeader(FORWARDED)?.Split(',').FirstOrDefault() ?? string.Empty;

        if (Debugger.IsAttached || string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = LOCAL_HOST_IP;
                request.AddHeader(FORWARDED, LOCAL_HOST_IP);
            }
        }
        return ipAddress;
    }

    /// <summary>
    /// Gets the request ID header value, creating one if it doesn't exist.
    /// </summary>
    internal static string GetRequestId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.CreateRequestId();
        return request.GetHeader(REQUEST_ID) ?? string.Empty;
    }

    /// <summary>
    /// Creates and sets a new request ID header if one does not exist.
    /// </summary>
    internal static void CreateRequestId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!string.IsNullOrEmpty(request.GetHeader(REQUEST_ID))) return;
        request.AddHeader(REQUEST_ID, $"{Guid.NewGuid():N}".Substring(0, 8) + $"-{DateTime.Now:ddMMyyyy-HHmmss}");
    }

    /// <summary>
    /// Gets the correlation ID header value, creating one if it doesn't exist.
    /// </summary>
    internal static string GetCorrelationId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.GetHeader(CORRELATION_ID) ?? string.Empty;
    }

    /// <summary>Gets the client ID from the request header.</summary>
    internal static string? GetClientId(this HttpRequest request) => request.GetHeader(CLIENT_ID);

    /// <summary>Adds a header to the request if it does not exist.</summary>
    internal static void AddHeader(this HttpRequest request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (!request.Headers.ContainsKey(key))
            request.Headers[key] = value;
    }

    /// <summary>Gets a header value by key if present; otherwise, null.</summary>
    internal static string? GetHeader(this HttpRequest request, string key)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(key);

        return request.Headers.TryGetValue(key, out var value) ? value.ToString() : null;
    }

}
