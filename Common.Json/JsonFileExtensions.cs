using System.Text;
using System.Text.Json;

namespace Common.Json;

/// <summary>
/// Extension methods for reading and writing JSON files.
/// </summary>
public static class JsonFileExtensions
{
    /// <summary>
    /// Reads a JSON file from the specified path and deserializes it into an object of type T.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize.</typeparam>
    /// <param name="path">The full file path to the JSON file.</param>
    /// <param name="options">Optional JSON serializer options.</param>
    /// <returns>The deserialized object of type T, or null if the content is empty.</returns>
    /// <exception cref="ArgumentException">Thrown when the path is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    public static T? JsonToObject<T>(string path, JsonSerializerOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"The file '{path}' was not found.");

        var json = File.ReadAllText(path, Encoding.UTF8);
        return JsonExtensions.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Serializes an object of type T to JSON and writes it to a file at the specified path.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="path">The full file path where the JSON will be saved.</param>
    /// <param name="data">The object to serialize.</param>
    /// <param name="options">Optional JSON serializer options.</param>
    /// <exception cref="ArgumentException">Thrown when the path is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the data object is null.</exception>
    public static void ObjectToJsonFile<T>(string path, T data, JsonSerializerOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        File.WriteAllText(path, JsonExtensions.Serialize(data, options), Encoding.UTF8);
    }
}
