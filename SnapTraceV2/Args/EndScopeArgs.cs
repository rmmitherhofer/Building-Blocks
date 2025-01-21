using SnapTraceV2.Services;

namespace SnapTraceV2.Args;

public class EndScopeArgs
{
    public LoggerService Logger { get; }
    public IDictionary<string, object> ScopeData { get; }

    public EndScopeArgs(LoggerService logger, Dictionary<string, object> scopeData)
    {
        ArgumentNullException.ThrowIfNull(nameof(logger), nameof(LoggerService));
        ArgumentNullException.ThrowIfNull(nameof(scopeData), nameof(Dictionary<string, object>));

        Logger = logger;
        ScopeData = scopeData;
    }
}
