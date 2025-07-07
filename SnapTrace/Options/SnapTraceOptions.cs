using SnapTrace.Formatters;

namespace SnapTrace.Options;

/// <summary>
/// Configuration options for SnapTrace logger.
/// </summary>
public class SnapTraceOptions
{
    /// <summary>
    /// Optional formatter for customizing log messages.
    /// </summary>
    public Func<FormatterArgs, string> Formatter { get; set; }
}
