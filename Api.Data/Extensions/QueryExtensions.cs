namespace Api.Data.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageNumber, int pageSize) 
        => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
}
