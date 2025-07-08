using Common.Extensions;
using Common.Json;
using Common.User.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Common.Http.Extensions;

public static class HttpRequestMessageExtensions
{
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
    /// Sets the JSON content of the HttpRequestMessage from the provided data object.
    /// </summary>
    /// <typeparam name="T">Type of data to serialize.</typeparam>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="data">Object to serialize as JSON content.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    public static void SetJsonContent<T>(this HttpRequestMessage request, T data, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        request.Content = JsonExtensions.SerializeContent(data, options);
    }

    /// <summary>
    /// Adds a header to the HttpRequestMessage if it does not already exist.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddHeader(this HttpRequestMessage request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (!request.Headers.Contains(key))
            request.Headers.Add(key, value);
    }

    /// <summary>
    /// Adds or updates a header in the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddOrUpdateHeader(this HttpRequestMessage request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (request.Headers.Contains(key))
            request.Headers.Remove(key);

        request.Headers.Add(key, value);
    }

    /// <summary>
    /// Retrieves the first value of a specific header from the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="key">Header name.</param>
    /// <returns>The header value if found; otherwise, null.</returns>
    public static string? GetHeader(this HttpRequestMessage request, string key)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (request.Headers.TryGetValues(key, out var values))
            return values.FirstOrDefault();

        return null;
    }

    /// <summary>
    /// Parses the query parameters from the HttpRequestMessage URI into a dictionary.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <returns>A dictionary of query parameter names and values.</returns>
    public static IDictionary<string, string> GetQueryParameters(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (request.RequestUri == null) return new Dictionary<string, string>();

        var query = request.RequestUri.Query;

        return Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query)
            .ToDictionary(k => k.Key, v => v.Value.ToString());
    }

    /// <summary>
    /// Creates a deep clone of the HttpRequestMessage, including headers and content.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance to clone.</param>
    /// <returns>A new HttpRequestMessage instance with copied data.</returns>
    public static HttpRequestMessage Clone(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            request.Content.CopyToAsync(ms).GetAwaiter().GetResult();
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        clone.Version = request.Version;

        return clone;
    }

    /// <summary>
    /// Adds the IP address from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderIpAddress(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var ip = _accessor.HttpContext?.Request.GetIpAddress();

        if (!string.IsNullOrEmpty(ip))
            request.AddHeader(HttpRequestExtensions.FORWARDED, ip);
    }

    /// <summary>
    /// Adds the user ID from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderUserId(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var userId = _accessor.HttpContext?.Request.GetUserId() ?? _accessor.HttpContext?.User?.GetId();

        if (!string.IsNullOrEmpty(userId))
            request.AddHeader(HttpRequestExtensions.USER_ID, userId);
    }

    /// <summary>
    /// Adds the correlation ID or request ID from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderCorrelationId(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var correlationId = _accessor.HttpContext?.Request.GetCorrelationId();

        if (string.IsNullOrEmpty(correlationId))
            correlationId = _accessor.HttpContext?.Request.GetRequestId();

        if (!string.IsNullOrEmpty(correlationId))
            request.AddHeader(HttpRequestExtensions.CORRELATION_ID, correlationId);
    }

    /// <summary>
    /// Adds the client ID (plus entry assembly name) from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderClientId(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var clientId = _accessor.HttpContext?.Request.GetClientId();

        clientId = string.Join(';', clientId, Assembly.GetEntryAssembly().GetName().Name);

        if (!string.IsNullOrEmpty(clientId))
            request.AddHeader(HttpRequestExtensions.CLIENT_ID, clientId);
    }

    /// <summary>
    /// Adds the user agent string from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderUserAgent(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var userAgent = _accessor.HttpContext?.Request.GetUserAgent();

        if (!string.IsNullOrEmpty(userAgent))
            request.AddHeader(HttpRequestExtensions.USER_AGENT, userAgent);
    }

    /// <summary>
    /// Adds the server hostname to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderServerHostName(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var podeName = Dns.GetHostName();

        if (!string.IsNullOrEmpty(podeName))
            request.AddHeader(HttpRequestExtensions.POD_NAME, podeName);
    }

    /// <summary>
    /// Adds the user account string from the current HTTP context to the HttpRequestMessage headers.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddHeaderUserAccount(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (_accessor is null) return;

        var userAccount = _accessor.HttpContext?.Request.GetUserAccount();

        if (!string.IsNullOrEmpty(userAccount))
            request.AddHeader(HttpRequestExtensions.USER_ACCOUNT, userAccount);
    }

    /// <summary>
    /// Adds all default headers (IP address, user ID, correlation ID, client ID, user agent, server hostname) to the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    public static void AddDefaultHeaders(this HttpRequestMessage request)
    {
        if (_accessor is null) return;

        request.AddHeaderIpAddress();
        request.AddHeaderUserId();
        request.AddHeaderCorrelationId();
        request.AddHeaderClientId();
        request.AddHeaderUserAgent();
        request.AddHeaderServerHostName();
        request.AddHeaderUserAccount();
    }

    /// <summary>
    /// Serializes the HttpRequestMessage headers to a JSON string.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <returns>JSON string representing the headers.</returns>
    public static string GetHeadersJsonFormat(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        return JsonExtensions.Serialize(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray()));
    }
}
