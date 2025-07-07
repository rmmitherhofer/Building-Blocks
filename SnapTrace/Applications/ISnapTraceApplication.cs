using Microsoft.AspNetCore.Http;
using SnapTrace.Models;

namespace SnapTrace.Applications;

/// <summary>
/// Defines a contract for notifying the SnapTrace system with request context information.
/// </summary>
public interface ISnapTraceApplication
{
    Task Notify(Snapshot snapshot);
}
