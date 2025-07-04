using SnapTrace.Configurations.Settings;
using System.Text.Json;

namespace SnapTrace.Extensions;

public class SensitiveDataMasker
{
    private readonly HashSet<string> _sensitiveKeys;

    public SensitiveDataMasker(SensitiveDataMaskerOptions options) => _sensitiveKeys = new HashSet<string>(options.SensitiveKeys, StringComparer.OrdinalIgnoreCase);

    public object? Mask(object? data)
    {
        if (data == null) return null;

        try
        {
            var json = JsonSerializer.Serialize(data);
            using var doc = JsonDocument.Parse(json);
            return MaskElement(doc.RootElement);
        }
        catch
        {
            return data;
        }
    }

    private object? MaskElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    var key = prop.Name;
                    if (_sensitiveKeys.Contains(key))
                    {
                        obj[key] = "***REDACTED***";
                    }
                    else
                    {
                        obj[key] = MaskElement(prop.Value);
                    }
                }
                return obj;

            case JsonValueKind.Array:
                return element.EnumerateArray().Select(MaskElement).ToList();

            case JsonValueKind.String: return element.GetString();
            case JsonValueKind.Number: return element.GetDouble();
            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.Null: return null;
            default: return null;
        }
    }
}