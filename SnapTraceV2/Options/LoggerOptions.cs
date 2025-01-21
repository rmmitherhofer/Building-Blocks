using SnapTraceV2.Args;
using SnapTraceV2.Factories;

namespace SnapTraceV2.Options;

public class LoggerOptions
{
    internal ILoggerFactory Factory { get; set; }
    public Func<FormatterArgs, string> Formatter { get; set; }
    public Action<BeginScopeArgs> OnBeginScope { get; set; }
    public Action<EndScopeArgs> OnEndScope { get; set; }
}
