using Common.Extensions;
using Common.Json;
using Common.User.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace Common.Http.Extensions;

/// <summary>
/// Extension methods for HttpClient to simplify header management and logging,
/// leveraging the current HTTP context via IHttpContextAccessor.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// The header key used to store the original request template.
    /// </summary>
    public static string X_REQUEST_TEMPLATE = "X-Request-Template";

    /// <summary>
    /// IHttpContextAccessor instance used to access the current HTTP context.
    /// </summary>
    private static IHttpContextAccessor? _accessor;

    /// <summary>
    /// Configures the IHttpContextAccessor for later use in extension methods.
    /// </summary>
    /// <param name="accessor">The IHttpContextAccessor to use.</param>
    public static void Configure(IHttpContextAccessor accessor) => _accessor = accessor;

    /// <summary>
    /// Adds a Bearer token to the Authorization header.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="token">Bearer token string.</param>
    public static void AddBearerToken(this HttpClient client, string token)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Adds a header to the HttpClient default request headers if it does not already exist.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddHeader(this HttpClient client, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (!client.DefaultRequestHeaders.Contains(key))
            client.DefaultRequestHeaders.Add(key, value);
    }

    /// <summary>
    /// Adds or updates a header in the HttpClient default request headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddOrUpdateHeader(this HttpClient client, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (client.DefaultRequestHeaders.Contains(key))
            client.DefaultRequestHeaders.Remove(key);

        client.DefaultRequestHeaders.Add(key, value);
    }

    /// <summary>
    /// Adds the IP address from the current HTTP context to the headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderIpAddress(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var ip = _accessor.HttpContext?.Request.GetIpAddress();

        if (!string.IsNullOrEmpty(ip))
            client.AddHeader(HttpRequestExtensions.FORWARDED, ip);
    }

    /// <summary>
    /// Adds the User ID from the current HTTP context to the headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderUserId(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var userId = _accessor.HttpContext?.Request.GetUserId() ?? _accessor.HttpContext?.User?.GetId();

        if (!string.IsNullOrEmpty(userId))
            client.AddHeader(HttpRequestExtensions.USER_ID, userId);
    }

    /// <summary>
    /// Adds the correlation ID from the current HTTP context to the headers,
    /// falling back to request ID if correlation ID is missing.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderCorrelationId(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var correlationId = _accessor.HttpContext?.Request.GetCorrelationId();

        if (string.IsNullOrEmpty(correlationId))
            correlationId = _accessor.HttpContext?.Request.GetRequestId();

        if (!string.IsNullOrEmpty(correlationId))
            client.AddHeader(HttpRequestExtensions.CORRELATION_ID, correlationId);
    }

    /// <summary>
    /// Adds the client ID (plus entry assembly name) from the current HTTP context to the headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderClientId(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var clientId = _accessor.HttpContext?.Request.GetClientId();

        clientId = string.Join(';', clientId, Assembly.GetEntryAssembly().GetName().Name);

        if (!string.IsNullOrEmpty(clientId))
            client.AddHeader(HttpRequestExtensions.CLIENT_ID, clientId);
    }

    /// <summary>
    /// Adds the User-Agent from the current HTTP context to the headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderUserAgent(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var userAgent = _accessor.HttpContext?.Request.GetUserAgent();

        if (!string.IsNullOrEmpty(userAgent))
            client.AddHeader(HttpRequestExtensions.USER_AGENT, userAgent);
    }

    /// <summary>
    /// Adds the server's host name to the headers.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderServerHostName(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var podeName = Dns.GetHostName();

        if (!string.IsNullOrEmpty(podeName))
            client.AddHeader(HttpRequestExtensions.POD_NAME, podeName);
    }

    /// <summary>
    /// Adds the User account from the current HTTP context to the headers
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddHeaderUserAccount(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        var userAccount = _accessor.HttpContext?.Request.GetUserAccount();

        if (!string.IsNullOrEmpty(userAccount))
            client.AddHeader(HttpRequestExtensions.USER_ACCOUNT, userAccount);
    }

    /// <summary>
    /// Adds default headers (IP, user ID, correlation ID, client ID, user agent, and server hostname).
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void AddDefaultHeaders(this HttpClient client)
    {
        if (_accessor is null) return;

        client.AddHeaderIpAddress();
        client.AddHeaderUserId();
        client.AddHeaderCorrelationId();
        client.AddHeaderClientId();
        client.AddHeaderUserAgent();
        client.AddHeaderServerHostName();
        client.AddHeaderUserAccount();
    }

    /// <summary>
    /// Returns the HttpClient default request headers serialized as a JSON string.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <returns>JSON string representing the headers.</returns>
    public static string GetHeadersJsonFormat(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        return JsonExtensions.Serialize(client.DefaultRequestHeaders.ToDictionary(h => h.Key, h => h.Value.ToArray()));
    }

    /// <summary>
    /// Adds the route template used in the request as a custom header (X-Request-Template).
    /// Useful for logging or tracing the original endpoint pattern.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="template">The original route template.</param>
    public static void AddHeaderRequestTemplate(this HttpClient client, string template)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (_accessor is null) return;

        if (!string.IsNullOrEmpty(template))
            client.AddOrUpdateHeader(X_REQUEST_TEMPLATE, template);
    }
}
