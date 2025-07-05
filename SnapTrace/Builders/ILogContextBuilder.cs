using Microsoft.AspNetCore.Http;
using SnapTrace.Models;

namespace SnapTrace.Builders;

/// <summary>
/// Defines a contract for building a <see cref="LogContextRequest"/> from HTTP context and related data.
/// </summary>
public interface ILogContextBuilder
{
    /// <summary>
    /// Adds the HTTP context to be used when building the log context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The builder instance.</returns>
    ILogContextBuilder WithHttpContext(HttpContext context);

    /// <summary>
    /// Adds the elapsed time in milliseconds for the current request.
    /// </summary>
    /// <param name="elapsedMs">The time in milliseconds.</param>
    /// <returns>The builder instance.</returns>
    ILogContextBuilder WithElapsedMilliseconds(long elapsedMs);

    /// <summary>
    /// Attaches any domain notifications (validation messages, etc.) to the log context.
    /// </summary>
    /// <returns>The builder instance.</returns>
    ILogContextBuilder WithNotifications();

    /// <summary>
    /// Attaches any exception captured during the request to the log context.
    /// </summary>
    /// <returns>The builder instance.</returns>
    ILogContextBuilder WithException();

    /// <summary>
    /// Builds the final <see cref="LogContextRequest"/> asynchronously with all collected data.
    /// </summary>
    /// <returns>The constructed log context object.</returns>
    Task<LogContextRequest> BuildAsync();
}
