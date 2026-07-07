using System.Collections.Concurrent;
using System.Reflection;
using Zypher.Comparison.Attributes;
using Zypher.Comparison.Metadata;

namespace Zypher.Comparison.Builders;

public static class ActivityMetadataBuilder
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, ActivityFieldMetadata>> Cache = new();
    private sealed record EntityFormatContext(string Format, string SourcePath);

    public static Dictionary<string, ActivityFieldMetadata> Build<T>()
    {
        return Cache.GetOrAdd(typeof(T), BuildForType);
    }

    private static Dictionary<string, ActivityFieldMetadata> BuildForType(Type rootType)
    {
        var map = new Dictionary<string, ActivityFieldMetadata>(StringComparer.Ordinal);
        AppendType(rootType, null, map, new HashSet<Type>(), null);
        return map;
    }

    private static void AppendType(
        Type type,
        string? prefix,
        IDictionary<string, ActivityFieldMetadata> map,
        ISet<Type> traversalStack,
        EntityFormatContext? inheritedFormatContext,
        bool enteredFromCollection = false)
    {
        if (!traversalStack.Add(type))
        {
            return;
        }

        var classFormat = type.GetCustomAttribute<ActivityEntityFormatAttribute>();
        var currentFormatContext = classFormat is null
            ? inheritedFormatContext
            : new EntityFormatContext(
                classFormat.Format,
                enteredFromCollection && inheritedFormatContext is not null
                    ? inheritedFormatContext.SourcePath
                    : (prefix ?? string.Empty));

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetMethod is null || property.GetMethod.GetParameters().Length > 0)
            {
                continue;
            }

            var propertyPath = string.IsNullOrEmpty(prefix)
                ? ToCamelCase(property.Name)
                : $"{prefix}.{ToCamelCase(property.Name)}";

            var fieldAttribute = property.GetCustomAttribute<ActivityFieldAttribute>();
            if (fieldAttribute is not null && !map.ContainsKey(propertyPath))
            {
                map[propertyPath] = new ActivityFieldMetadata
                {
                    Entity = fieldAttribute.Entity ?? string.Empty,
                    Field = fieldAttribute.Field ?? string.Empty,
                    EntityFormat = currentFormatContext?.Format,
                    EntityFormatSourcePath = currentFormatContext?.SourcePath
                };
            }

            var propertyType = UnwrapNullable(property.PropertyType);
            var collectionElementType = GetCollectionElementType(propertyType);
            if (collectionElementType is not null)
            {
                var elementType = UnwrapNullable(collectionElementType);
                if (IsComplexType(elementType))
                {
                    AppendType(elementType, propertyPath, map, traversalStack, currentFormatContext, enteredFromCollection: true);
                }

                continue;
            }

            if (IsComplexType(propertyType))
            {
                AppendType(propertyType, propertyPath, map, traversalStack, currentFormatContext);
            }
        }

        traversalStack.Remove(type);
    }

    private static bool IsComplexType(Type type)
    {
        if (type == typeof(string)) return false;
        if (type.IsPrimitive || type.IsEnum) return false;

        if (type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan) ||
            type == typeof(Guid))
        {
            return false;
        }

        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type)) return false;

        return type.IsClass || type.IsValueType;
    }

    private static Type UnwrapNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (type == typeof(string)) return null;

        if (type.IsArray) return type.GetElementType();

        if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(type)) return null;

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length == 1) return genericArgs[0];
        }

        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableInterface?.GetGenericArguments()[0];
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0])) return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
