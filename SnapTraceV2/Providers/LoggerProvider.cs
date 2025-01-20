using Microsoft.Extensions.Logging;
using SnapTraceV2.Adapters;
using SnapTraceV2.Options;

namespace SnapTraceV2.Providers;

public class LoggerProvider : ILoggerProvider
{
    private readonly LoggerOptions _options;

    public LoggerProvider() : this(null) { }
    public LoggerProvider(LoggerOptions options) => _options = options;

    public ILogger CreateLogger(string categoryName) => new LoggerAdapter(_options, categoryName);

    public void Dispose() { }
}
