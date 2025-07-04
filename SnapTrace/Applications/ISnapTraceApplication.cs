using Microsoft.AspNetCore.Http;

namespace SnapTrace.Applications;

public interface ISnapTraceApplication
{
    Task Notify(HttpContext context, long elapsedMilliseconds);
}
