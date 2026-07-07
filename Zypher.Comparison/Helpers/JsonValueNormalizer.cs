using System.Text.Json;

namespace Zypher.Comparison.Helpers;

public static class JsonValueNormalizer
{
    public static string? GetValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => NormalizeString(element.GetString()),
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Object => element.GetRawText(),
            JsonValueKind.Array => element.GetRawText(),
            _ => element.GetRawText()
        };
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
