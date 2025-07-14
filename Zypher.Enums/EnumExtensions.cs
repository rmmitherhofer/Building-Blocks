using System.Collections.Concurrent;
using System.Reflection;
using Zypher.Enums.Resolvers;

namespace Zypher.Enums;

/// <summary>
/// Provides extension methods for working with enum descriptions using custom resolvers.
/// </summary>
public static class EnumExtensions
{
    private static readonly ConcurrentDictionary<Enum, string> _descriptionCache = new();
    private static readonly ConcurrentDictionary<Type, Dictionary<string, object>> _lookupCache = new();

    private static readonly CompositeEnumDescriptionResolver _defaultResolver = new(
        new DescriptionAttributeResolver(),
        new EnumMemberAttributeResolver(),
        new DisplayAttributeResolver(),
        new NameFallbackResolver()
    );

    /// <summary>
    /// Gets the description of the enum value using the provided or default resolver.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">The enum value.</param>
    /// <param name="resolver">Optional custom resolver.</param>
    /// <returns>Description string of the enum value.</returns>
    public static string GetDescription<T>(this T value, IEnumDescriptionResolver? resolver = null) where T : Enum
    {
        if (_descriptionCache.TryGetValue(value, out var cached))
            return cached;

        var type = typeof(T);
        var name = Enum.GetName(type, value);
        if (name == null) return string.Empty;

        var field = type.GetField(name);
        var description = (resolver ?? _defaultResolver).GetDescription(field!) ?? name;

        _descriptionCache.TryAdd(value, description);
        return description;
    }

    /// <summary>
    /// Converts a description string back to its enum value.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="description">The description to match.</param>
    /// <param name="resolver">Optional custom resolver.</param>
    /// <returns>The matching enum value.</returns>
    /// <exception cref="ArgumentException">Thrown if description is not found.</exception>
    public static T FromDescription<T>(this string description, IEnumDescriptionResolver? resolver = null) where T : struct, Enum
    {
        if (TryFromDescription(description, out T result, resolver))
            return result;

        throw new ArgumentException($"The description '{description}' was not found in enum '{typeof(T).Name}'.", nameof(description));
    }

    /// <summary>
    /// Tries to convert a description string to its enum value.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="description">The description to match.</param>
    /// <param name="result">The resulting enum value if found.</param>
    /// <param name="resolver">Optional custom resolver.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static bool TryFromDescription<T>(string description, out T result, IEnumDescriptionResolver? resolver = null) where T : struct, Enum
    {
        var type = typeof(T);
        var key = description.ToLowerInvariant();

        if (!_lookupCache.TryGetValue(type, out var map))
        {
            map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var enumValue = (T)field.GetValue(null)!;
                var desc = (resolver ?? _defaultResolver).GetDescription(field);
                if (!string.IsNullOrWhiteSpace(desc))
                    map.TryAdd(desc.ToLowerInvariant(), enumValue);
            }
            _lookupCache.TryAdd(type, map);
        }

        if (map.TryGetValue(key, out var val))
        {
            result = (T)val;
            return true;
        }

        result = default;
        return false;
    }
}
