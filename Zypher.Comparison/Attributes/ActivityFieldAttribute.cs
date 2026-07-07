namespace Zypher.Comparison.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ActivityFieldAttribute(string? entity, string? field) : Attribute
{
    public string? Entity { get; } = entity;
    public string? Field { get; } = field;
}
