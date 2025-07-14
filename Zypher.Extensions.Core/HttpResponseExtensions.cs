using Microsoft.AspNetCore.Http;

namespace Zypher.Extensions.Core;

/// <summary>
/// Extension methods for <see cref="HttpResponse"/> to manage headers and correlation IDs.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Header key for correlation ID.
    /// </summary>
    public const string CORRELATION_ID = "X-Correlation-ID";

    /// <summary>
    /// Sets the correlation ID header in the response, copying from the request correlation ID or request ID.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> to set the header on.</param>
    public static void SetCorrelationId(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));

        var correlationId = response.HttpContext.Request.GetCorrelationId() ?? response.HttpContext.Request.GetRequestId();
        response.AddHeader(CORRELATION_ID, correlationId);
    }

    /// <summary>
    /// Adds a header to the response if it does not already exist.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> to add the header to.</param>
    /// <param name="key">The header name.</param>
    /// <param name="value">The header value.</param>
    public static void AddHeader(this HttpResponse response, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        if (!response.Headers.ContainsKey(key))
            response.Headers[key] = value;
    }

    /// <summary>
    /// Adds or updates a header in the response.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> to update the header in.</param>
    /// <param name="key">The header name.</param>
    /// <param name="value">The header value.</param>
    public static void AddOrUpdateHeader(this HttpResponse response, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        if (response.Headers.ContainsKey(key))
            response.Headers.Remove(key);

        response.Headers[key] = value;
    }

    /// <summary>
    /// Gets the value of a response header by key.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> to get the header from.</param>
    /// <param name="key">The header name.</param>
    /// <returns>The header value if found; otherwise, <c>null</c>.</returns>
    public static string? GetHeader(this HttpResponse response, string key)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        return response.Headers.TryGetValue(key, out var value) ? value.ToString() : null;
    }
}
