using System.Linq.Expressions;

namespace Api.Data.Extensions;

public static class OrderByExtensions
{
    private static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return default;

        propertyName = propertyName.Replace("-", "").Replace("+", "");

        var param = Expression.Parameter(typeof(TSource), "x");
        Expression expression = Expression.Convert(Expression.Property(param, propertyName), typeof(object));

        return Expression.Lambda<Func<TSource, object>>(expression, param);
    }

    private static Func<TSource, object> GetFunc<TSource>(string propertyName) 
        => GetExpression<TSource>(propertyName).Compile();

    public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string? propertyName)
    {
        if(string.IsNullOrEmpty(propertyName)) return source;

        if (!string.IsNullOrEmpty(propertyName) && propertyName.StartsWith("-"))
            return source.OrderByDescending(GetFunc<TSource>(propertyName));

        return source.OrderBy(GetFunc<TSource>(propertyName));
    }

    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return source;

        if (!string.IsNullOrEmpty(propertyName) && propertyName.StartsWith("-"))
            return source.OrderByDescending(GetExpression<TSource>(propertyName));

        return source.OrderBy(GetExpression<TSource>(propertyName));
    }
}
