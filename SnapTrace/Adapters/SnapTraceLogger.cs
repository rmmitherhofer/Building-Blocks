using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SnapTrace.Extensions;
using SnapTrace.Formatters;
using SnapTrace.Models;

namespace SnapTrace.Adapters;

public class SnapTraceLogger : ILogger
{
    private readonly string _category;
    private readonly LoggerOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string LogsKey = "__SnapTrace_Logger_Logs__";

    public SnapTraceLogger(LoggerOptions options, string category, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(LoggerOptions));

        _options = options;
        _category = category;
        _httpContextAccessor = httpContextAccessor;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null) return;

        var message = formatter(state, exception);

        if (_options.Formatter != null)
        {
            FormatterArgs formatterArgs = new(new FormatterArgs.CreateOptions
            {
                State = state,
                Exception = exception,
                DefaultValue = message,
            });

            string custom = _options.Formatter.Invoke(formatterArgs);

            message = string.IsNullOrEmpty(custom) ? message : custom;
        }

        var logEntry = new LogEntry(_category, logLevel, message).Member();

        var context = _httpContextAccessor.HttpContext;

        if (context == null) return;

        if (!context.Items.TryGetValue(LogsKey, out var obj) || obj is not List<LogEntry> logs)
        {
            logs = new List<LogEntry>();
            context.Items[LogsKey] = logs;
        }
        logs.Add(logEntry);
    }

    public static IEnumerable<LogEntry> GetLogsForCurrentRequest(HttpContext context)
    {
        if (context.Items.TryGetValue(LogsKey, out var obj) && obj is IEnumerable<LogEntry> logs)
            return logs;

        return [];
    }
}


[ProviderAlias("SnapTrace")]
public class SnapTraceLoggerProvider : ILoggerProvider
{
    private readonly LoggerOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SnapTraceLoggerProvider(LoggerOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SnapTraceLogger(_options, categoryName, _httpContextAccessor);
    }
    public void Dispose() { }
}

public class LoggerOptions
{
    public Func<FormatterArgs, string> Formatter { get; set; }
}