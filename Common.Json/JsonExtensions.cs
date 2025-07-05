using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json;

/// <summary>
/// Utility methods for JSON serialization and deserialization with default configuration.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Default content type used for JSON: "application/json".
    /// </summary>
    public const string CONTENT_TYPE = "application/json";

    /// <summary>
    /// Default options for JSON serialization.
    /// </summary>
    public static readonly JsonSerializerOptions SerializeDefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    /// <summary>
    /// Default options for JSON deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions DeserializeDefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    /// <summary>
    /// Serializes an object into JSON content (StringContent).
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <param name="options">Custom serialization options (optional).</param>
    /// <returns>JSON content as StringContent.</returns>
    public static StringContent SerializeContent(object data, JsonSerializerOptions? options = null) =>
        new(Serialize(data, options), Encoding.UTF8, CONTENT_TYPE);

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <param name="options">Serialization options (optional).</param>
    /// <returns>JSON string.</returns>
    public static string Serialize(object data, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Serialize(data, options ?? SerializeDefaultOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to serialize the object to JSON.", ex);
        }
    }

    /// <summary>
    /// Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="content">The JSON content string.</param>
    /// <param name="options">Deserialization options (optional).</param>
    /// <returns>Deserialized object or null if input is empty.</returns>
    public static T? Deserialize<T>(string content, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(content)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(content, options ?? DeserializeDefaultOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON to type '{typeof(T).Name}': Invalid JSON format. {ex.Message}", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException($"Deserialization to type '{typeof(T).Name}' is not supported. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error while deserializing JSON to type '{typeof(T).Name}'. {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Tries to deserialize a JSON string into an object of the specified type, without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="content">The JSON content string.</param>
    /// <param name="result">The deserialized object or null if failed.</param>
    /// <param name="options">Deserialization options (optional).</param>
    /// <returns>True if deserialization succeeds; otherwise, false.</returns>
    public static bool TryDeserialize<T>(string content, out T? result, JsonSerializerOptions? options = null)
    {
        try
        {
            result = Deserialize<T>(content, options);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
