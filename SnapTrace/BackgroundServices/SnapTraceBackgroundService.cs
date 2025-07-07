using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnapTrace.Applications;
using SnapTrace.Queues;

namespace SnapTrace.BackgroundServices;

/// <summary>
/// Background service responsible for processing log snapshots from the SnapTrace queue
/// and sending them asynchronously to the SnapTrace API.
/// </summary>
public class SnapTraceBackgroundService(ISnapTraceQueue queue, ISnapTraceApplication snapTrace, ILogger<SnapTraceBackgroundService> logger) : BackgroundService
{
    private readonly ILogger<SnapTraceBackgroundService> _logger = logger;
    private readonly ISnapTraceQueue _queue = queue;
    private readonly ISnapTraceApplication _snapTrace = snapTrace;

    /// <summary>
    /// Continuously reads log snapshots from the queue and forwards them to the SnapTrace application
    /// for processing and delivery. Handles exceptions to ensure the background service remains alive.
    /// </summary>
    /// <param name="stoppingToken">Token used to signal service shutdown.</param>

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var snapshot in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _snapTrace.Notify(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SnapTraceBackgroundService)}]:{ex.Message} - {ex.StackTrace}");
            }
        }
    }
}
