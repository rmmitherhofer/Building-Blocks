namespace Api.Data.Extensions;

public static class QueryExtensions
{
    /// <summary>
    /// Applies pagination to the query by skipping and taking the specified number of records.
    /// </summary>
    /// <typeparam name="T">The type of the queryable data.</typeparam>
    /// <param name="query">The source IQueryable to paginate.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <returns>An IQueryable containing only the records for the requested page.</returns>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}

