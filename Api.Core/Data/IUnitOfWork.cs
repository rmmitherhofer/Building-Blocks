using Common.Core.Enums;

namespace Api.Core.Data;

/// <summary>
/// Defines a unit of work contract for managing transaction consistency.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The result is a tuple:
    /// - <c>bool</c>: Indicates whether the commit was successful.
    /// - <see cref="OperationType"/>: The type of operation performed (Added, Modified, Deleted, etc.).
    /// </returns>
    Task<(bool, OperationType)> Commit();
}
