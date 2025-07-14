using System.Reflection;

namespace Zypher.Enums.Resolvers;

/// <summary>
/// Combines multiple <see cref="IEnumDescriptionResolver"/> implementations into one,
/// returning the first non-null and non-empty description found.
/// </summary>
public class CompositeEnumDescriptionResolver : IEnumDescriptionResolver
{
    private readonly IEnumDescriptionResolver[] _resolvers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeEnumDescriptionResolver"/> class
    /// with the specified list of description resolvers.
    /// </summary>
    /// <param name="resolvers">The resolvers to evaluate in order.</param>
    public CompositeEnumDescriptionResolver(params IEnumDescriptionResolver[] resolvers) =>
        _resolvers = resolvers;

    /// <summary>
    /// Gets the description for a given enum field by delegating to the inner resolvers.
    /// Returns the first non-null and non-empty result.
    /// </summary>
    /// <param name="field">The field info representing the enum member.</param>
    /// <returns>The description string, or <c>null</c> if none is found.</returns>
    public string? GetDescription(FieldInfo field)
    {
        foreach (var resolver in _resolvers)
        {
            var desc = resolver.GetDescription(field);
            if (!string.IsNullOrWhiteSpace(desc))
                return desc;
        }
        return null;
    }
}
