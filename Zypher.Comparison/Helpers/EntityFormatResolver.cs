using System.Text;
using System.Text.Json;

namespace Zypher.Comparison.Helpers;

public static class EntityFormatResolver
{
    public static string ResolveEntity(string fallbackEntity, string? format, string? sourcePath, string normalizedPath, string lookupPath, JsonElement? newState, JsonElement? oldState)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return fallbackEntity;
        }

        var source = sourcePath ?? string.Empty;
        var actualSourcePath = MapLookupPathToActualPath(source, lookupPath, normalizedPath);

        if (TryResolveFormattedEntity(format!, actualSourcePath, newState, out var resolvedFromNew))
        {
            return NormalizeValue(resolvedFromNew, fallbackEntity);
        }

        if (TryResolveFormattedEntity(format!, actualSourcePath, oldState, out var resolvedFromOld))
        {
            return NormalizeValue(resolvedFromOld, fallbackEntity);
        }

        return fallbackEntity;
    }

    public static string MapLookupPathToActualPath(string sourceLookupPath, string lookupPath, string actualPath)
    {
        if (string.IsNullOrEmpty(sourceLookupPath))
        {
            return string.Empty;
        }

        var sourceSegments = sourceLookupPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var lookupSegments = lookupPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var actualSegments = actualPath.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (sourceSegments.Length > lookupSegments.Length || lookupSegments.Length > actualSegments.Length)
        {
            return sourceLookupPath;
        }

        var result = new List<string>(sourceSegments.Length);
        for (var i = 0; i < sourceSegments.Length; i++)
        {
            if (!string.Equals(sourceSegments[i], lookupSegments[i], StringComparison.Ordinal))
            {
                return sourceLookupPath;
            }

            result.Add(actualSegments[i]);
        }

        return string.Join('.', result);
    }

    private static bool TryResolveFormattedEntity(string format, string contextPath, JsonElement? state, out string entityName)
    {
        entityName = string.Empty;
        if (!state.HasValue)
        {
            return false;
        }

        if (!TryGetElementByPath(state.Value, contextPath, out var contextElement))
        {
            return false;
        }

        entityName = ApplyTemplate(format, contextElement);
        return true;
    }

    private static bool TryGetElementByPath(JsonElement root, string path, out JsonElement result)
    {
        result = root;
        if (string.IsNullOrEmpty(path))
        {
            return true;
        }

        foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (!TryReadSegment(result, segment, out result))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryReadSegment(JsonElement current, string segment, out JsonElement result)
    {
        result = current;
        if (string.IsNullOrEmpty(segment))
        {
            return false;
        }

        var bracketIndex = segment.IndexOf('[');
        var propertyName = bracketIndex >= 0 ? segment[..bracketIndex] : segment;

        if (!string.IsNullOrEmpty(propertyName))
        {
            if (current.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (!TryGetPropertyCaseInsensitive(current, propertyName, out current))
            {
                return false;
            }
        }

        var position = bracketIndex;
        while (position >= 0 && position < segment.Length)
        {
            var closeIndex = segment.IndexOf(']', position + 1);
            if (closeIndex < 0)
            {
                return false;
            }

            var indexText = segment.Substring(position + 1, closeIndex - position - 1);
            if (!int.TryParse(indexText, out var index) || index < 0)
            {
                return false;
            }

            if (current.ValueKind != JsonValueKind.Array || current.GetArrayLength() <= index)
            {
                return false;
            }

            current = current[index];
            position = segment.IndexOf('[', closeIndex + 1);
        }

        result = current;
        return true;
    }

    private static bool TryGetPropertyCaseInsensitive(JsonElement element, string propertyName, out JsonElement result)
    {
        if (element.TryGetProperty(propertyName, out result))
        {
            return true;
        }

        var camelName = ToCamelCase(propertyName);
        return element.TryGetProperty(camelName, out result);
    }

    private static string ApplyTemplate(string format, JsonElement context)
    {
        var output = new StringBuilder(format.Length + 16);
        var position = 0;

        while (position < format.Length)
        {
            var open = format.IndexOf('{', position);
            if (open < 0)
            {
                output.Append(format, position, format.Length - position);
                break;
            }

            output.Append(format, position, open - position);
            var close = format.IndexOf('}', open + 1);
            if (close < 0)
            {
                output.Append(format, open, format.Length - open);
                break;
            }

            var token = format.Substring(open + 1, close - open - 1).Trim();
            output.Append(ResolveTemplateToken(context, token));
            position = close + 1;
        }

        return output.ToString();
    }

    private static string ResolveTemplateToken(JsonElement context, string token)
    {
        if (string.IsNullOrEmpty(token) || context.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        if (!TryGetPropertyCaseInsensitive(context, token, out var value))
        {
            return string.Empty;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Null => string.Empty,
            JsonValueKind.String => value.GetString() ?? string.Empty,
            _ => value.ToString()
        };
    }

    private static string NormalizeValue(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
