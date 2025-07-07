using Microsoft.Extensions.Options;
using SnapTrace.Builders;
using SnapTrace.Configurations.Settings;
using SnapTrace.Enums;
using SnapTrace.HttpServices;
using SnapTrace.Models;

namespace SnapTrace.Applications;

/// <summary>
/// Implementation of <see cref="ISnapTraceApplication"/> responsible for building and sending log context data
/// to the SnapTrace API based on runtime snapshot information.
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
    /// Builds a complete log context from the provided snapshot and sends it to the SnapTrace API,
    /// if logging is enabled via configuration.
    /// </summary>
    /// <param name="snapshot">The snapshot containing request, response, exception, and other contextual data.</param>

    public async Task Notify(Snapshot snapshot)
    {
        if (_settings.ExecutionMode == SnapTraceExecutionMode.Disabled) return;

        LogContextRequest log = null;

        switch (_settings.ExecutionMode)
        {
            case SnapTraceExecutionMode.ExceptionsOnly:
                log = _builder.WithSnapTraceLogSnapshot(snapshot)
                    .WithException()
                    .Build();
                break;

            case SnapTraceExecutionMode.NotificationsAndExceptions:
                log = _builder.WithSnapTraceLogSnapshot(snapshot)
                    .WithNotifications()
                    .WithException()
                    .Build();
                break;

            case SnapTraceExecutionMode.Full:
                log = _builder.WithSnapTraceLogSnapshot(snapshot)
                    .WithLogEntries()
                    .WithNotifications()
                    .WithException()
                    .Build();
                break;
        }
        await _httpService.Flush(log);
    }
}
