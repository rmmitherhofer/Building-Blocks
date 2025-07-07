using SnapTrace.Models;
using System.Threading.Channels;

namespace SnapTrace.Queues;

/// <summary>
/// Represents a background queue for storing <see cref="Snapshot"/> instances to be processed asynchronously.
/// </summary>
public interface ISnapTraceQueue
{
    /// <summary>
    /// Enqueues a <see cref="Snapshot"/> to be processed by the SnapTrace background service.
    /// </summary>
    /// <param name="snapshot">The snapshot containing request, response, and context data.</param>
    void Enqueue(Snapshot snapshot);

    /// <summary>
    /// Gets the channel reader that provides access to the enqueued <see cref="Snapshot"/> instances.
    /// </summary>
    ChannelReader<Snapshot> Reader { get; }
}
