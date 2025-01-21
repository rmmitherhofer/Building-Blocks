using Microsoft.Extensions.Logging;
using SnapTraceV2.Args;
using SnapTraceV2.Extensions;
using SnapTraceV2.Options;
using SnapTraceV2.Services;

namespace SnapTraceV2.Adapters;

public class LoggerAdapter : ILogger
{
    private readonly string? _category;
    private readonly LoggerOptions _options;

    public LoggerAdapter(LoggerOptions options, string? category = null)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(LoggerOptions));

        _options = options;
        _category = category;
    }

    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        var logger = GetLogger();

        return new SnapTraceScope<TState>(state, logger, _options);
    }

    bool ILogger.IsEnabled(LogLevel logLevel) => true;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LoggerService logger = GetLogger();

        string message = formatter(state, exception);

        if (_options.Formatter != null)
        {
            FormatterArgs formatterArgs = new(new FormatterArgs.CreateOptions
            {
                State = state,
                Exception = exception,
                DefaultValue = message,
                Logger = logger
            });

            string custom = _options.Formatter.Invoke(formatterArgs);

            message = string.IsNullOrEmpty(custom) ? message : custom;
        }

        switch (logLevel)
        {
            case LogLevel.Trace:
            default:
                logger.Trace(message);
                break;
            case LogLevel.Debug:
                logger.Debug(message);
                break;

            case LogLevel.Information:
                logger.Info(message);
                break;

            case LogLevel.Warning:
                logger.Warn(message);
                break;

            case LogLevel.Error:
                logger.Error(message);
                break;

            case LogLevel.Critical:
                logger.Critical(message);
                break;

            case LogLevel.None:
                break;
        }
    }

    internal LoggerService GetLogger()
    {
        Factories.ILoggerFactory factory = _options.Factory ?? LoggerService.Factory;
        return factory.Get(_category);
    }
}
