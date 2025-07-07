using System.Linq.Expressions;
using System.Reflection;

namespace Api.Data.Extensions;

/// <summary>
/// Extension methods for dynamic ordering by one or multiple property names on IEnumerable and IQueryable.
/// Supports nested properties and case-insensitive property names.
/// </summary>
public static class OrderByExtensions
{
    private static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

        propertyName = propertyName.Trim();
        if (propertyName.StartsWith("+") || propertyName.StartsWith("-"))
            propertyName = propertyName.Substring(1);

        var parameter = Expression.Parameter(typeof(TSource), "x");
        Expression propertyAccess = parameter;

        foreach (var prop in propertyName.Split('.'))
        {
            var propInfo = propertyAccess.Type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, prop, StringComparison.OrdinalIgnoreCase));

            if (propInfo == null)
                throw new ArgumentException($"Property '{prop}' not found on type '{propertyAccess.Type.Name}'.");

            propertyAccess = Expression.Property(propertyAccess, propInfo);
        }

        if (propertyAccess.Type.IsValueType)
            propertyAccess = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<TSource, object>>(propertyAccess, parameter);
    }

    private static Func<TSource, object> GetFunc<TSource>(string propertyName)
        => GetExpression<TSource>(propertyName).Compile();

    /// <summary>
    /// Orders an IEnumerable by one or multiple properties dynamically.
    /// Multiple properties are separated by commas.
    /// Prefix "-" indicates descending order, "+" or no prefix means ascending.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements.</typeparam>
    /// <param name="source">Enumerable source.</param>
    /// <param name="orderByProperties">Comma-separated property names with optional +/- prefix.</param>
    /// <returns>Ordered IEnumerable.</returns>
    public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string? orderByProperties)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(IEnumerable<TSource>));

        if (string.IsNullOrWhiteSpace(orderByProperties)) return source;

        var properties = orderByProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedEnumerable<TSource>? orderedQuery = null;

        foreach (var prop in properties)
        {
            var trimmedProp = prop.Trim();
            bool descending = trimmedProp.StartsWith("-");
            var propName = trimmedProp.TrimStart('+', '-');

            if (orderedQuery == null)
            {
                orderedQuery = descending
                    ? source.OrderByDescending(GetFunc<TSource>(propName))
                    : source.OrderBy(GetFunc<TSource>(propName));
            }
            else
            {
                orderedQuery = descending
                    ? orderedQuery.ThenByDescending(GetFunc<TSource>(propName))
                    : orderedQuery.ThenBy(GetFunc<TSource>(propName));
            }
        }

        return orderedQuery ?? source;
    }

    /// <summary>
    /// Orders an IQueryable by one or multiple properties dynamically.
    /// Multiple properties are separated by commas.
    /// Prefix "-" indicates descending order, "+" or no prefix means ascending.
    /// </summary>
    /// <typeparam name="TSource">Type of source elements.</typeparam>
    /// <param name="source">Queryable source.</param>
    /// <param name="orderByProperties">Comma-separated property names with optional +/- prefix.</param>
    /// <returns>Ordered IQueryable.</returns>
    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string? orderByProperties)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(IQueryable<TSource>));

        if (string.IsNullOrWhiteSpace(orderByProperties)) return source;

        var properties = orderByProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
        IOrderedQueryable<TSource>? orderedQuery = null;

        foreach (var prop in properties)
        {
            var trimmedProp = prop.Trim();
            bool descending = trimmedProp.StartsWith("-");
            var propName = trimmedProp.TrimStart('+', '-');

            if (orderedQuery == null)
            {
                orderedQuery = descending
                    ? source.OrderByDescending(GetExpression<TSource>(propName))
                    : source.OrderBy(GetExpression<TSource>(propName));
            }
            else
            {
                orderedQuery = descending
                    ? orderedQuery.ThenByDescending(GetExpression<TSource>(propName))
                    : orderedQuery.ThenBy(GetExpression<TSource>(propName));
            }
        }

        return orderedQuery ?? source;
    }
}
