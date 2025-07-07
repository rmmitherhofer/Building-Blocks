using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SnapTrace.Adapters;
using SnapTrace.Options;

namespace SnapTrace.Providers;

/// <summary>
/// Logger provider for SnapTrace logger.
/// </summary>
[ProviderAlias("SnapTrace")]
public class SnapTraceLoggerProvider : ILoggerProvider
{
    private readonly SnapTraceOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapTraceLoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The logger options.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public SnapTraceLoggerProvider(SnapTraceOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Creates a new SnapTrace logger.
    /// </summary>
    /// <param name="categoryName">The logger category name.</param>
    /// <returns>The created logger.</returns>
    public ILogger CreateLogger(string categoryName) => new SnapTraceLogger(_options, categoryName, _httpContextAccessor);

    /// <summary>
    /// Disposes the provider. No-op.
    /// </summary>
    public void Dispose() { }
}
