using Microsoft.AspNetCore.Http;

namespace SnapTrace.Applications;

/// <summary>
/// Defines a contract for notifying the SnapTrace system with request context information.
/// </summary>
public interface ISnapTraceApplication
{
    /// <summary>
    /// Sends notification to SnapTrace with the given HTTP context and request elapsed time.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="elapsedMilliseconds">The time taken to process the request in milliseconds.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Notify(HttpContext context, long elapsedMilliseconds);
}
