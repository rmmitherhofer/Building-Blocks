namespace Common.Core.DomainObjects;

/// <summary>
/// Base class for domain entities with a unique identifier and audit properties.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets the date and time the entity was registered/created.
    /// </summary>
    public DateTime RegistrationDate { get; protected set; }

    /// <summary>
    /// Gets the date and time the entity was last changed, if any.
    /// </summary>
    public DateTime? DateChanged { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class
    /// with a new unique identifier.
    /// </summary>
    protected Entity() => Id = Guid.NewGuid();

    #region Comparisons

    /// <summary>
    /// Determines whether the specified object is equal to the current entity,
    /// comparing by Id.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo))
            return true;

        if (compareTo is null)
            return false;

        return Id.Equals(compareTo.Id);
    }

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="a">The first entity.</param>
    /// <param name="b">The second entity.</param>
    /// <returns>True if both entities are equal; otherwise, false.</returns>
    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="a">The first entity.</param>
    /// <param name="b">The second entity.</param>
    /// <returns>True if both entities are not equal; otherwise, false.</returns>
    public static bool operator !=(Entity a, Entity b) => !(a == b);

    /// <summary>
    /// Gets the hash code for the entity.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => (GetType().GetHashCode() * 971) + Id.GetHashCode();

    /// <summary>
    /// Returns a string that represents the current entity.
    /// </summary>
    /// <returns>A string representation of the entity.</returns>
    public override string ToString() => $"{GetType().Name} [Id={Id}]";

    #endregion
}
