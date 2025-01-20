using SnapTrace.Http;

namespace SnapTrace.Interfaces
{
    public interface ILogListener
    {
        ILogListenerInterceptor Interceptor { get; }

        void OnBeginRequest(HttpRequest httpRequest);
        void OnMessage(LogMessage message);
        void OnFlush(FlushLogArgs args);
    }
}
