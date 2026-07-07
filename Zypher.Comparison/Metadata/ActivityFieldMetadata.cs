namespace Zypher.Comparison.Metadata;

public sealed class ActivityFieldMetadata
{
    public string Entity { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string? EntityFormat { get; set; }
    public string? EntityFormatSourcePath { get; set; }
}
