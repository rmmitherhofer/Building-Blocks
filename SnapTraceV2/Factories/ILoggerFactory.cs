using SnapTraceV2.Services;

namespace SnapTraceV2.Factories;

public interface ILoggerFactory
{
    LoggerService Get(string? category = null, string? url = null);
    IEnumerable<LoggerService> GetAll();
}
