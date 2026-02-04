using System.Net.Http.Headers;

namespace Zypher.Http.Extensions;

/// <summary>
/// Extension methods for HttpClient to simplify header management and logging,
/// leveraging the current HTTP context via IHttpContextAccessor.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Adds a Bearer token to the Authorization header.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="token">Bearer token string.</param>
    public static void AddBearerToken(this HttpClient client, string token)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        client.DefaultRequestHeaders.Authorization = null;

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
            client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
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

        client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
    }
}
