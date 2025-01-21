using SnapTraceV2.Models.Http;
using SnapTraceV2.Services;

namespace SnapTraceV2.Models.Logger;

public class LoggerDataContainer : IDisposable
{
    internal bool _disposed;

    private List<LogMessage> _messages;
    private List<Exception> _exceptions;

    internal DateTime DateTimeCreated { get; }
    public HttpProperties HttpProperties { get; private set; }
    public IEnumerable<LogMessage> LogMessages => _messages;
    public IEnumerable<Exception> Exceptions => _exceptions;
    internal FilesContainer FilesContainer { get; }
    internal LoggerProperties LoggerProperties { get; }

    internal LoggerDataContainer(LoggerService logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _messages = [];
        _exceptions = [];

        DateTimeCreated = DateTime.UtcNow;
        FilesContainer = new (logger);
        LoggerProperties = new ();
    }

    internal void SetHttpProperties(HttpProperties httpProperties)
    {
        ArgumentNullException.ThrowIfNull(httpProperties, nameof(Http.HttpProperties));

        HttpProperties = httpProperties;
    }

    internal void Add(LogMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(LogMessage));

        _messages.Add(message);
    }

    internal void Add(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(Exception));

        _exceptions.Add(exception);
    }

    public void Dispose()
    {
        FilesContainer.Dispose();

        _disposed = true;
    }
}
