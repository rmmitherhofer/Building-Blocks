using SnapTraceV2.Args;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.NotifyListeners;

public interface ILogListener
{
    ILogListenerInterceptor Interceptor { get; }

    void OnBeginRequest(HttpRequest httpRequest);
    void OnMessage(LogMessage message);
    void OnFlush(FlushLogArgs args);
}
