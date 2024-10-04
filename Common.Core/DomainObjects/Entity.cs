namespace Core.DomainObjects;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime RegistrationDate { get; protected set; }
    public DateTime? DateChanged { get; protected set; }

    protected Entity() => Id = Guid.NewGuid();

    #region Comparações
    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo))
            return true;

        if (compareTo is null)
            return false;

        return Id.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b) => !(a == b);
    public override int GetHashCode() => (GetType().GetHashCode() * 971) + Id.GetHashCode();
    public override string ToString() => $"{GetType().Name} [Id={Id}]";
    #endregion
}
