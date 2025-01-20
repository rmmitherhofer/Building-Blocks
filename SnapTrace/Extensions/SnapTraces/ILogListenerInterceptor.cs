using SnapTrace.Http;
using SnapTrace.Interfaces;

namespace SnapTrace
{
    public interface ILogListenerInterceptor
    {
        bool ShouldLog(HttpRequest httpRequest, ILogListener listener);
        bool ShouldLog(LogMessage message, ILogListener listener);
        bool ShouldLog(FlushLogArgs args, ILogListener listener);
    }
}
