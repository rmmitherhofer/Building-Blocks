using System.Reflection;
using System.Text;
using System.Web;
using Zypher.Http.Attributes;

namespace Zypher.Http.Extensions;

/// <summary>
/// Provides extension methods for converting objects and dictionaries into query string representations.
/// </summary>
public static class QueryStringExtensions
{
    /// <summary>
    /// Converts an object's public properties into a query string.
    /// Only non-null properties are included.
    /// Custom query parameter names can be provided using the <see cref="QueryStringPropertyAttribute"/>.
    /// </summary>
    /// <param name="source">The object whose properties will be converted to query parameters.</param>
    /// <returns>A query string starting with '?', or an empty string if no valid parameters exist.</returns>
    public static string ToQueryString(this object source)
    {
        if (source is null) return string.Empty;

        var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keyValuePairs = new List<KeyValuePair<string, string>>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(source);
            if (value == null) continue;

            // Ignora coleções e objetos complexos
            var type = prop.PropertyType;
            if (!IsSimpleType(type)) continue;

            var attr = prop.GetCustomAttribute<QueryStringPropertyAttribute>();
            var name = attr?.Name ?? prop.Name;

            keyValuePairs.Add(new KeyValuePair<string, string>(
                HttpUtility.UrlEncode(ToCamelCase(name)),
                HttpUtility.UrlEncode(value.ToString())
            ));
        }

        if (!keyValuePairs.Any()) return string.Empty;

        var sb = new StringBuilder("?");
        sb.Append(string.Join("&", keyValuePairs.Select(kv => $"{kv.Key}={kv.Value}")));
        return sb.ToString();
    }

    /// <summary>
    /// Converts a dictionary of key-value pairs into a query string.
    /// Keys are converted to camelCase and null values are ignored.
    /// </summary>
    /// <param name="dict">The dictionary containing query parameter names and values.</param>
    /// <returns>A query string starting with '?', or an empty string if the dictionary is null or empty.</returns>
    public static string ToQueryString(this IDictionary<string, object?> dict)
    {
        if (dict == null || !dict.Any()) return string.Empty;

        var sb = new StringBuilder("?");
        var query = dict
            .Where(kv => kv.Value != null)
            .Select(kv =>
                $"{HttpUtility.UrlEncode(ToCamelCase(kv.Key))}={HttpUtility.UrlEncode(kv.Value!.ToString())}"
            );

        sb.Append(string.Join("&", query));
        return sb.ToString();
    }

    /// <summary>
    /// Converts the first character of a string to lowercase (camelCase).
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input string in camelCase format.</returns>
    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
    /// <summary>
    /// Determines whether the specified <see cref="Type"/> is considered a simple type.
    /// Simple types include primitives, enums, strings, decimals, GUIDs, DateTime, and nullable versions of these types.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns><c>true</c> if the type is simple; otherwise, <c>false</c>.</returns>
    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            type.IsEnum ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(Guid) ||
            Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type)!);
    }

    /// <summary>
    /// Generates a query string template using property names as placeholders.
    /// Only simple types (primitives, string, enum, Guid, DateTime, etc) are included.
    /// </summary>
    /// <param name="source">The object whose property names will be used as placeholders.</param>
    /// <returns>A query string in the format "?param1={Property1}&param2={Property2}".</returns>
    public static string ToQueryStringTemplate(this object source)
    {
        if (source is null) return string.Empty;

        var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keyValuePairs = new List<string>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(source);
            if (value == null) continue;  // só ignora null

            var type = prop.PropertyType;
            if (!IsSimpleType(type)) continue;

            var attr = prop.GetCustomAttribute<QueryStringPropertyAttribute>();
            var name = attr?.Name ?? prop.Name;

            var encodedName = HttpUtility.UrlEncode(ToCamelCase(name));
            var placeholder = $"{{{prop.Name}}}";
            keyValuePairs.Add($"{encodedName}={placeholder}");
        }

        if (!keyValuePairs.Any()) return string.Empty;

        return "?" + string.Join("&", keyValuePairs);
    }
}
