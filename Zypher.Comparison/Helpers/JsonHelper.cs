using System.Text.Json;

namespace Zypher.Comparison.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static JsonElement ToJsonElement<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj, _options);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}