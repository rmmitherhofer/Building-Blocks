using Common.Core.Enums;

namespace Api.Core;

public interface IUnitOfWork
{
    Task<bool> Commit();

    Task<(bool, OperationType)> CommitDetailed();
}
