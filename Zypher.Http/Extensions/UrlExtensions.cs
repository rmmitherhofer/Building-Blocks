using Zypher.Http.Attributes;

namespace Zypher.Http.Extensions;

/// <summary>
/// Provides extension methods for appending query strings to URLs.
/// </summary>
public static class UrlExtensions
{
    /// <summary>
    /// Appends the given object's properties as query string parameters to the URL.
    /// Properties with null values are ignored.
    /// </summary>
    /// <param name="url">The base URL to which the query string will be appended.</param>
    /// <param name="queryParams">
    /// An object whose public properties will be converted into query string parameters.
    /// Supports custom parameter names via <see cref="QueryStringPropertyAttribute"/>.
    /// </param>
    /// <returns>The URL with the appended query string.</returns>
    public static string AddQueryString(this string url, object queryParams)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;

        var query = queryParams.ToQueryString();
        if (string.IsNullOrEmpty(query)) return url;

        return url.Contains("?") ? url + "&" + query.TrimStart('?') : url + query;
    }

    /// <summary>
    /// Appends the given object's properties as query string parameters to the URI.
    /// Properties with null values are ignored.
    /// </summary>
    /// <param name="uri">The base <see cref="Uri"/> to which the query string will be appended.</param>
    /// <param name="queryParams">
    /// An object whose public properties will be converted into query string parameters.
    /// Supports custom parameter names via <see cref="QueryStringPropertyAttribute"/>.
    /// </param>
    /// <returns>A new <see cref="Uri"/> with the appended query string.</returns>
    public static Uri AddQueryString(this Uri uri, object queryParams)
    {
        var builder = new UriBuilder(uri);
        var query = queryParams.ToQueryString();

        if (string.IsNullOrEmpty(query)) return uri;

        if (!string.IsNullOrWhiteSpace(builder.Query))
            builder.Query = builder.Query.TrimStart('?') + "&" + query.TrimStart('?');
        else
            builder.Query = query.TrimStart('?');

        return builder.Uri;
    }
}