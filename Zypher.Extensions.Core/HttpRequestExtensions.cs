using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;

namespace Zypher.Extensions.Core;

/// <summary>
/// Provides extension methods for <see cref="HttpRequest"/> to simplify common HTTP request operations.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>X-User-ID header key.</summary>
    public const string USER_ID = "X-User-ID";
    /// <summary>X-User-Name header key.</summary>
    public const string USER_NAME = "X-User-Name";
    /// <summary>X-User-Name header key.</summary>
    public const string USER_ACCOUNT = "X-User-Account";
    /// <summary>X-User-Name-Code header key.</summary>
    public const string USER_ACCOUNT_CODE = "X-User-Account-Code";
    /// <summary>X-User-Name header key.</summary>
    public const string USER_DOCUMENT = "X-User-Document";
    /// <summary>User-Agent header key.</summary>
    public const string USER_AGENT = "User-Agent";
    /// <summary>X-Forwarded-For header key.</summary>
    public const string FORWARDED = "X-Forwarded-For";
    /// <summary>X-Requested-With header key.</summary>
    public const string REQUESTED_WITH = "X-Requested-With";

    /// <summary>X-Request-ID header key.</summary>
    public const string REQUEST_ID = "X-Request-ID";
    /// <summary>X-Correlation-ID header key.</summary>
    public const string CORRELATION_ID = "X-Correlation-ID";
    /// <summary>X-Client-ID header key.</summary>
    public const string CLIENT_ID = "X-Client-ID";
    /// <summary>X-Pod-Name header key.</summary>
    public const string POD_NAME = "X-Pod-Name";

    private const string LOCAL_HOST_IP = "127.0.0.1";

    /// <summary>
    /// Gets the user ID from the request header.
    /// </summary>
    public static string? GetUserId(this HttpRequest request) => request.GetHeader(USER_ID);

    /// <summary>
    /// Gets the user name from the request header.
    /// </summary>
    public static string? GetUserName(this HttpRequest request) => request.GetHeader(USER_NAME);
    /// <summary>
    /// Gets the user account from the request header.
    /// </summary>
    public static string? GetUserAccount(this HttpRequest request) => request.GetHeader(USER_ACCOUNT);
    /// <summary>
    /// Gets the user account code from the request header.
    /// </summary>
    public static string? GetUserAccountCode(this HttpRequest request) => request.GetHeader(USER_ACCOUNT_CODE);
    /// <summary>
    /// Gets the user document from the request header.
    /// </summary>
    public static string? GetUserDocument(this HttpRequest request) => request.GetHeader(USER_DOCUMENT);

    /// <summary>
    /// Gets the user agent string from the request header.
    /// </summary>
    public static string? GetUserAgent(this HttpRequest request) => request.GetHeader(USER_AGENT);

    /// <summary>
    /// Gets the IP address of the client making the request, considering forwarded headers.
    /// Defaults to localhost if no IP found.
    /// </summary>
    public static string GetIpAddress(this HttpRequest request)
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
    public static string GetRequestId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.CreateRequestId();
        return request.GetHeader(REQUEST_ID) ?? string.Empty;
    }

    /// <summary>
    /// Creates and sets a new request ID header if one does not exist.
    /// </summary>
    public static void CreateRequestId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!string.IsNullOrEmpty(request.GetHeader(REQUEST_ID))) return;
        request.AddHeader(REQUEST_ID, $"{Guid.NewGuid():N}".Substring(0, 8) + $"-{DateTime.Now:ddMMyyyy-HHmmss}");
    }

    /// <summary>
    /// Gets the correlation ID header value, creating one if it doesn't exist.
    /// </summary>
    public static string GetCorrelationId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.CreateCorrelationId();
        return request.GetHeader(CORRELATION_ID) ?? string.Empty;
    }

    /// <summary>
    /// Creates and sets a new correlation ID header if one does not exist,
    /// using the request ID as the correlation ID.
    /// </summary>
    public static void CreateCorrelationId(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!string.IsNullOrEmpty(request.GetHeader(CORRELATION_ID))) return;
        request.AddHeader(CORRELATION_ID, request.GetRequestId());
    }

    /// <summary>Gets the client ID from the request header.</summary>
    public static string? GetClientId(this HttpRequest request) => request.GetHeader(CLIENT_ID);

    /// <summary>Gets the pod name from the request header.</summary>
    public static string? GetPodName(this HttpRequest request) => request.GetHeader(POD_NAME);

    /// <summary>Gets the content type (without parameters) from the request header.</summary>
    public static string? GetContentType(this HttpRequest request)
        => request.ContentType?.Split(';').FirstOrDefault()?.Trim();

    /// <summary>Returns true if the content type is JSON.</summary>
    public static bool IsJsonContent(this HttpRequest request)
        => request.GetContentType()?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>Returns true if the request was made via AJAX.</summary>
    public static bool IsAjaxRequest(this HttpRequest request)
        => request.GetHeader(REQUESTED_WITH) == "XMLHttpRequest";

    /// <summary>Returns the query parameters as a dictionary.</summary>
    public static IDictionary<string, string> GetQueryDictionary(this HttpRequest request)
        => request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

    /// <summary>Gets the full URL of the request.</summary>
    public static string GetFullUrl(this HttpRequest request)
        => $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

    /// <summary>Adds a header to the request if it does not exist.</summary>
    public static void AddHeader(this HttpRequest request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (!request.Headers.ContainsKey(key))
            request.Headers[key] = value;
    }

    /// <summary>Adds or updates a header in the request.</summary>
    public static void AddOrUpdateHeader(this HttpRequest request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(key);
        request.Headers[key] = value;
    }

    /// <summary>Checks if the connection is secure (HTTPS).</summary>
    public static bool IsSecureConnection(this HttpRequest request)
        => request.IsHttps || string.Equals(request.Scheme, "https", StringComparison.OrdinalIgnoreCase);

    /// <summary>Gets the HTTP method in uppercase.</summary>
    public static string GetHttpMethod(this HttpRequest request) => request.Method.ToUpperInvariant();

    /// <summary>Gets route info in the format "controller/action" if available.</summary>
    public static string? GetRouteInfo(this HttpRequest request)
    {
        var routeValues = request.HttpContext.GetRouteData()?.Values;
        if (routeValues == null) return null;

        var controller = routeValues.TryGetValue("controller", out var c) ? c?.ToString() : null;
        var action = routeValues.TryGetValue("action", out var a) ? a?.ToString() : null;

        return controller != null && action != null ? $"{controller}/{action}" : null;
    }

    /// <summary>Reads the request body as a string asynchronously and resets the stream position.</summary>
    public static async Task<string> ReadBodyAsStringAsync(this HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        request.Body.Position = 0;
        return body;
    }

    /// <summary>Checks whether the request has a body to read.</summary>
    public static bool HasBody(this HttpRequest request) => request.ContentLength > 0 || request.Body.CanRead;

    /// <summary>Checks whether the request has form content type.</summary>
    public static bool IsFormContent(this HttpRequest request) => request.HasFormContentType;

    /// <summary>Determines if the request appears to be a webhook by header presence.</summary>
    public static bool IsWebhookRequest(this HttpRequest request)
        => request.Headers.ContainsKey("X-Hub-Signature")
        || request.Headers.ContainsKey("X-GitHub-Event")
        || request.Headers.ContainsKey("X-Gitlab-Event");

    /// <summary>Gets the approximate UTC time when the request started processing.</summary>
    public static DateTimeOffset GetRequestStartTime(this HttpRequest request) => DateTimeOffset.UtcNow;

    /// <summary>Gets a header value by key if present; otherwise, null.</summary>
    public static string? GetHeader(this HttpRequest request, string key)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(key);

        return request.Headers.TryGetValue(key, out var value) ? value.ToString() : null;
    }
}
