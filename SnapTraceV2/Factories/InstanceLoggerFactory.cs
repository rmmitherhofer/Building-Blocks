using SnapTraceV2.Services;

namespace SnapTraceV2.Factories;

internal class InstanceLoggerFactory(LoggerService logger) : ILoggerFactory
{
    private readonly LoggerService _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public LoggerService Get(string? category = null, string? url = null) => _logger;

    public IEnumerable<LoggerService> GetAll() => [_logger];
}
