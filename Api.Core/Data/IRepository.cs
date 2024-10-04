using Core.DomainObjects;

namespace Api.Core;

public interface IRepository<TEntity> : IDisposable where TEntity : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity?> GetById(Guid id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
