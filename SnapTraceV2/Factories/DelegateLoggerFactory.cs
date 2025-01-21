using SnapTraceV2.Services;
using System.Collections.Concurrent;

namespace SnapTraceV2.Factories;

internal class DelegateLoggerFactory(Func<string, string, LoggerService> loggerServiceFn) : ILoggerFactory
{
    private readonly ConcurrentDictionary<Guid, LoggerService> _loggersServices = new();
    private readonly Func<string, string, LoggerService> _loggerServiceFn = loggerServiceFn ?? throw new ArgumentNullException(nameof(loggerServiceFn));

    public LoggerService Get(string? category = null, string? url = null)
    {
        LoggerService service = _loggerServiceFn.Invoke(category, url);

        service ??= new LoggerService(category, url);

        return _loggersServices.GetOrAdd(service.Id, service);
    }

    public IEnumerable<LoggerService> GetAll()
    {
        List<LoggerService> loggers = [];

        foreach (Guid id in _loggersServices.Keys)
        {
            if (_loggersServices.TryGetValue(id, out LoggerService? loggerService) && loggerService is not null)
                loggers.Add(loggerService);
        }
        return loggers;
    }
}
