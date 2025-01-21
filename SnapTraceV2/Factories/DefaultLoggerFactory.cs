using SnapTraceV2.Services;

namespace SnapTraceV2.Factories;

internal class DefaultLoggerFactory : ILoggerFactory
{
    public LoggerService Get(string? category = null, string? url = null) => new(category, url);
    public IEnumerable<LoggerService> GetAll() => [];
}
