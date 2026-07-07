namespace Zypher.Comparison.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ActivityEntityFormatAttribute(string format) : Attribute
{
    public string Format { get; } = format;
}
