using Microsoft.AspNetCore.Http;

namespace Common.Extensions;

public static class HttpResponseExtensions
{
    public const string CORRELATION_ID = "X-Correlation-ID";
    public static void SetCorrelationId(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));

        response.AddHeader(CORRELATION_ID, response.HttpContext.GetCorrelationId() ?? response.HttpContext.GetRequestId());
    }

    public static void AddHeader(this HttpResponse response, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        if (response.Headers.ContainsKey(key))
            response.Headers.Remove(key);

        response.Headers.TryAdd(key, value);
    }
    public static string GetHeader(this HttpResponse response, string key)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        if (response.Headers.TryGetValue(key, out var value))
            return value.ToString();

        return null;
    }
}
