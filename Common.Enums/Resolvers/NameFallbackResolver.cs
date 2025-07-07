using System.Reflection;

namespace Common.Enums.Resolvers;

/// <summary>
/// Fallback resolver that returns the name of the enum field as its description.
/// </summary>
public class NameFallbackResolver : IEnumDescriptionResolver
{
    /// <summary>
    /// Gets the name of the enum field as the description.
    /// </summary>
    /// <param name="field">The field info representing the enum member.</param>
    /// <returns>
    /// The name of the field.
    /// </returns>
    public string? GetDescription(FieldInfo field) => field.Name;
}
