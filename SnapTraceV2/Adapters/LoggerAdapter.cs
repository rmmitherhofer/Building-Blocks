using Microsoft.Extensions.Logging;
using SnapTraceV2.Options;

namespace SnapTraceV2.Adapters;

public class LoggerAdapter : ILogger
{
    private readonly string? _category;
    private readonly LoggerOptions _options;

    public LoggerAdapter(LoggerOptions options, string? category = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(LoggerOptions), "Not null");
        _category = category;
    }

    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        return default(IDisposable);
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ///Armazenar esses logs em uma lista da sessao ativa
        string message = formatter(state, exception);

        switch (logLevel)
        {
            case Microsoft.Extensions.Logging.LogLevel.Debug:
                Console.WriteLine(message);
                break;

            case Microsoft.Extensions.Logging.LogLevel.Information:
                Console.WriteLine(message);
                break;

            case Microsoft.Extensions.Logging.LogLevel.Warning:
                Console.WriteLine(message);
                break;

            case Microsoft.Extensions.Logging.LogLevel.Error:
                Console.WriteLine(message);
                break;

            case Microsoft.Extensions.Logging.LogLevel.Critical:
                Console.WriteLine(message);
                break;

            case Microsoft.Extensions.Logging.LogLevel.None:
                break;

            default:
                Console.WriteLine(message);
                break;
        }
    }
}
