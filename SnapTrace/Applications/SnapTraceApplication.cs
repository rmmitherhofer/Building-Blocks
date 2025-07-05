using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SnapTrace.Builders;
using SnapTrace.Configurations.Settings;
using SnapTrace.HttpServices;

namespace SnapTrace.Applications;

/// <summary>
/// Implementation of <see cref="ISnapTraceApplication"/> that builds and sends log context to SnapTrace API.
/// </summary>
public class SnapTraceApplication : ISnapTraceApplication
{
    private readonly ILogContextBuilder _builder;
    private readonly ISnapTraceHttpService _httpService;
    private readonly SnapTraceSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapTraceApplication"/> class.
    /// </summary>
    /// <param name="httpService">The HTTP service to send log data to SnapTrace.</param>
    /// <param name="options">Configuration options for SnapTrace.</param>
    /// <param name="builder">Builder used to construct the log context.</param>
    public SnapTraceApplication(
        ISnapTraceHttpService httpService,
        IOptions<SnapTraceSettings> options,
        ILogContextBuilder builder)
    {
        _httpService = httpService;
        _settings = options.Value;
        _builder = builder;
    }

    /// <summary>
    /// Notifies SnapTrace by building a log context from the current HTTP request and sending it asynchronously.
    /// </summary>
    /// <param name="context">The HTTP context of the request.</param>
    /// <param name="elapsedMilliseconds">The time in milliseconds the request took to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Notify(HttpContext context, long elapsedMilliseconds)
    {
        if (!_settings.TurnOnLog) return;

        var log = await _builder
            .WithHttpContext(context)
            .WithElapsedMilliseconds(elapsedMilliseconds)
            .WithNotifications()
            .WithException()
            .BuildAsync();

        Task.Run(() => _httpService.Flush(log));
    }
}
