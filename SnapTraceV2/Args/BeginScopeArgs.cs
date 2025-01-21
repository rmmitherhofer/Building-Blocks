using SnapTraceV2.Services;

namespace SnapTraceV2.Args;

public class BeginScopeArgs
{
    public object State { get; }
    public LoggerService Logger { get; }
    public IDictionary<string, object> ScopeData { get; }

    public BeginScopeArgs(object state, LoggerService logger, Dictionary<string, object> scopeData)
    {
        ArgumentNullException.ThrowIfNull(nameof(logger), nameof(LoggerService));
        ArgumentNullException.ThrowIfNull(nameof(scopeData), nameof(Dictionary<string, object>));

        State = state;
        Logger = logger;
        ScopeData = scopeData;
    }
}
