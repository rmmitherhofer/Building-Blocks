using SnapTrace.Models;

namespace SnapTrace.HttpServices;

/// <summary>
/// Interface for sending logs to the SnapTrace service.
/// </summary>
public interface ISnapTraceHttpService
{
    /// <summary>
    /// Sends a complete log context to the SnapTrace API.
    /// </summary>
    /// <param name="log">The structured log context containing request, response, and diagnostics data.</param>
    Task Flush(LogContextRequest log);
}
