using SnapTraceV2.Args;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.NotifyListeners;

public interface ILogListenerInterceptor
{
    bool ShouldLog(HttpRequest httpRequest, ILogListener listener);
    bool ShouldLog(LogMessage message, ILogListener listener);
    bool ShouldLog(FlushLogArgs args, ILogListener listener);
}