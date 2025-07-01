using Microsoft.AspNetCore.Http;

namespace Common.Extensions;

public static class HttpRequestExtensions
{
    public static void AddHeader(this HttpRequest request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequest));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        if (request.Headers.ContainsKey(key))
            request.Headers.Remove(key);

        request.Headers.TryAdd(key, value);
    }

    public static string GetHeader(this HttpRequest request, string key)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequest));

        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        if (request.Headers.TryGetValue(key, out var value))
            return value.ToString();

        return null;
    }
}
