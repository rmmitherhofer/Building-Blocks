using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SnapTraceV2.Extensions;

internal static class HttpRequestExtensions
{
    public static List<KeyValuePair<string, string?>> ToKeyValuePair(this IRequestCookieCollection collection)
    {
        if (collection is null) return [];

        List<KeyValuePair<string, string?>> result = [];

        foreach (string key in collection.Keys)
        {
            if (string.IsNullOrWhiteSpace(key)) continue;

            string? value = collection[key];

            result.Add(new KeyValuePair<string, string?>(key, value));
        }
        return result;
    }

    public static List<KeyValuePair<string, string?>> ToKeyValuePair(this IHeaderDictionary collection)
    {
        if (collection is null) return [];

        List<KeyValuePair<string, string?>> result = [];

        foreach (string key in collection.Keys)
        {
            if (string.IsNullOrWhiteSpace(key)) continue;

            string? value = collection[key];

            result.Add(new KeyValuePair<string, string?>(key, value));
        }
        return result;
    }

    public static List<KeyValuePair<string, string?>> ToKeyValuePair(this IFormCollection collection)
    {
        if (collection is null) return [];

        List<KeyValuePair<string, string?>> result = [];

        foreach (string key in collection.Keys)
        {
            if (string.IsNullOrWhiteSpace(key)) continue;

            string? value = collection[key];

            result.Add(new KeyValuePair<string, string?>(key, value));
        }
        return result;
    }

    public static List<KeyValuePair<string, string?>> ToKeyValuePair(this IQueryCollection collection)
    {
        if (collection is null) return [];

        List<KeyValuePair<string, string?>> result = [];

        foreach (string key in collection.Keys)
        {
            if (string.IsNullOrWhiteSpace(key)) continue;

            string? value = collection[key];

            result.Add(new KeyValuePair<string, string?>(key, value));
        }
        return result;
    }

    public static List<KeyValuePair<string, string>> ToKeyValuePair(this ClaimsIdentity claimsIdentity)
    {
        if (claimsIdentity is null || claimsIdentity.Claims is null) return [];

        return claimsIdentity.Claims.Where(p => !string.IsNullOrWhiteSpace(p.Type))
            .Select(p => new KeyValuePair<string, string>(p.Type, p.Value))
            .ToList();
    }
}