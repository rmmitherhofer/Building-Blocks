using System.Text.Json;

namespace SnapTraceV2.Json;

internal class SystemTextJsonDeserializeAndFlatten
{
    public IEnumerable<KeyValuePair<string, object>> DeserializeAndFlatten(JsonDocument document)
    {
        ArgumentNullException.ThrowIfNull(document, nameof(JsonDocument));

        List<KeyValuePair<string, object>> result = [];

        FillDictionary(result, document.RootElement, null);

        return result;
    }

    private void FillDictionary(List<KeyValuePair<string, object>> dict, JsonElement element, string? path)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (JsonProperty prop in element.EnumerateObject())
                {
                    if (!string.IsNullOrWhiteSpace(prop.Name))
                        FillDictionary(dict, prop.Value, Join(path, "." + prop.Name));
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (JsonElement item in element.EnumerateArray())
                {
                    FillDictionary(dict, item, Join(path, "[]"));
                    index++;
                }
                break;

            default:
                object value = GetValue(element);
                dict.Add(new KeyValuePair<string, object>(path, value));
                break;
        }
    }

    private object? GetValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Undefined or JsonValueKind.Null => null,
            JsonValueKind.True or JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDouble(),
            _ => null,
        };
    }

    private string Join(string prefix, string path) => (prefix + path).Trim().Trim('.');
}
