namespace Api.Core;

public interface IUnitOfWork
{
    Task<bool> Commit();
}
