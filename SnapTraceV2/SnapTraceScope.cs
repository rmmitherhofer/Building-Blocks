using SnapTraceV2.Args;
using SnapTraceV2.Options;
using SnapTraceV2.Services;

namespace SnapTraceV2;

internal class SnapTraceScope<TState> : IDisposable
{
    private readonly LoggerService _logger;
    private readonly LoggerOptions _options;
    private readonly Dictionary<string, object> _scopeData;

    public SnapTraceScope(TState state, LoggerService logger, LoggerOptions options)
    {
        ArgumentNullException.ThrowIfNull(nameof(logger), nameof(LoggerService));
        ArgumentNullException.ThrowIfNull(nameof(options), nameof(LoggerOptions));

        _logger = logger;
        _options = options;
        _scopeData = [];

        _options.OnBeginScope?.Invoke(new BeginScopeArgs(state, logger, _scopeData));
    }


    public void Dispose() => _options.OnEndScope?.Invoke(new EndScopeArgs(_logger, _scopeData));
}
