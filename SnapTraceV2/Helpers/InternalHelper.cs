using Microsoft.AspNetCore.Http.Headers;
using SnapTraceV2.Services;
using System.Text.RegularExpressions;

namespace SnapTraceV2.Helpers;

internal static class InternalHelper
{
    public static string? GetMachineName()
    {
        string? name = null;

        try
        {
            name =
                Environment.GetEnvironmentVariable("CUMPUTERNAME") ??
                Environment.GetEnvironmentVariable("HOSTNAME") ??
                System.Net.Dns.GetHostName();
        }
        catch { }

        return name;
    }

    public static void WrapInTryCatch(Action fn)
    {
        ArgumentNullException.ThrowIfNull(fn, nameof(Action));

        try
        {
            fn.Invoke();
        }
        catch (Exception ex)
        {
            InternalLogHelper.LogException(ex);
        }
    }

    public static T? WrapInTryCatch<T>(Func<T> fn)
    {
        ArgumentNullException.ThrowIfNull(fn, nameof(Func<T>));

        try
        {
            return fn.Invoke();
        }
        catch (Exception ex)
        {
            InternalLogHelper.LogException(ex);
        }

        return default;
    }

    public static Uri GenerateUri(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new Uri(Constants.DefaultBaseUrl, UriKind.Absolute);

        url = url.Trim().Trim('/');

        url = Constants.UrlRegex.Replace(url, "-");

        url = Regex.Replace(url, @"\-+", "-").Trim('-');

        string scheme = "http";
        string host = "application";
        string pathAndQuery = url;

        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            scheme = uri.Scheme;
            host = uri.Host;
            pathAndQuery = uri.PathAndQuery;
        }

        pathAndQuery = Regex.Replace(pathAndQuery, @"/+", @"/").Trim('/');

        if (pathAndQuery.Contains("?"))
        {
            string path = pathAndQuery.Substring(0, pathAndQuery.IndexOf("?")).Trim('/');
            string query = pathAndQuery.Substring(pathAndQuery.IndexOf("?") + 1).Trim('/');

            pathAndQuery = $"{path}?{query}";
        }

        url = $"{scheme}://{host}/";
        if (!string.IsNullOrEmpty(pathAndQuery))
        {
            url = $"{url}{pathAndQuery}";
        }

        return new Uri(url, UriKind.Absolute);
    }

    public static bool CanReadRequestInputStream(IEnumerable<KeyValuePair<string, string>> requestHeaders)
    {
        if (requestHeaders is null) return false;

        string[] allowedContentTypes = Constants.ReadInputStreamContentTypes;

        string contentType = requestHeaders.FirstOrDefault(p => string.Compare(p.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value;

        if (string.IsNullOrEmpty(contentType)) return false;

        contentType = contentType.Trim().ToLowerInvariant();

        return allowedContentTypes.Any(p => contentType.Contains(p));
    }

    public static bool CanReadResponseBody(IEnumerable<KeyValuePair<string, string>> responseHeaders)
    {
        if (responseHeaders is null) return false;

        string[] allowedContentTypes = Constants.ReadResponseBodyContentTypes;

        string contentType = responseHeaders.FirstOrDefault(p => string.Compare(p.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value;

        if (string.IsNullOrEmpty(contentType)) return false;

        contentType = contentType.Trim().ToLowerInvariant();

        return allowedContentTypes.Any(p => contentType.Contains(p));
    }

    public static bool? GetExplicitLogResponseBody(IEnumerable<LoggerService> loggers)
    {
        ArgumentNullException.ThrowIfNull(loggers, nameof(IEnumerable<LoggerService>));

        LoggerService? defaultLogger = loggers.FirstOrDefault(p => p.Category == Constants.DefaultLoggerCategory);

        bool? result = defaultLogger?.DataContainer.LoggerProperties.ExplicitLogResponseBody;

        return result ?? (loggers.FirstOrDefault(p => p.DataContainer.LoggerProperties.ExplicitLogResponseBody.HasValue == true)?.DataContainer.LoggerProperties.ExplicitLogResponseBody);
    }

    public static string GenerateResponseFileName(IEnumerable<KeyValuePair<string, string>> responseHeaders)
    {
        string defaultResponseFileName = "Response.txt";

        if (responseHeaders is null) return defaultResponseFileName;

        string contentType = responseHeaders.FirstOrDefault(p => string.Compare(p.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value ?? string.Empty;
        contentType = contentType.ToLowerInvariant();

        if (contentType.Contains("/json")) return "Response.json";

        if (contentType.Contains("/xml")) return "Response.xml";

        if (contentType.Contains("/html")) return "Response.html";

        return defaultResponseFileName;
    }


    public static Uri BuildUri(string baseUrl, string resource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl, nameof(baseUrl));

        resource = CombineUriParts(baseUrl, resource);

        return new Uri(resource, UriKind.Absolute);
    }

    internal static string CombineUriParts(params string[] uriParts)
    {
        var uri = string.Empty;

        if (uriParts?.Any() == true)
        {
            uriParts = uriParts.Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();

            char[] trimChars = { '\\', '/' };

            uri = (uriParts[0] ?? string.Empty).TrimEnd(trimChars);

            for (var i = 1; i < uriParts.Length; i++)
            {
                uri = $"{uri.TrimEnd(trimChars)}/{(uriParts[i] ?? string.Empty).TrimStart(trimChars)}";
            }
        }
        return uri;
    }
}
