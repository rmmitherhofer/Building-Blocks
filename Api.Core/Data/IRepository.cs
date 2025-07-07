using Common.Core.DomainObjects;

namespace Api.Core.Data;

/// <summary>
/// Defines the contract for a generic repository to manage aggregate root entities.
/// </summary>
/// <typeparam name="TEntity">Type of entity that implements <see cref="IAggregateRoot"/>.</typeparam>
public interface IRepository<TEntity> : IDisposable where TEntity : IAggregateRoot
{
    /// <summary>
    /// Gets the unit of work associated with this repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing a collection of entities.</returns>
    Task<IEnumerable<TEntity>> GetAll();

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <returns>A task that represents the asynchronous operation, containing the entity or null if not found.</returns>
    Task<TEntity?> GetById(Guid id);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);
}
