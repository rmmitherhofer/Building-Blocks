using SnapTraceV2.Decorators;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models.Http;

namespace SnapTraceV2.NotifyListeners;

internal static class NotifyBeginRequest
{
    public static void Notify(HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest, nameof(HttpRequest));

        foreach (LogListenerDecorator decorator in (List<LogListenerDecorator>)SnapTraceOptionsConfiguration.Listeners.GetAll())
            InternalHelper.WrapInTryCatch(() => Notify(httpRequest, decorator));
    }

    private static void Notify(HttpRequest httpRequest, LogListenerDecorator decorator)
    {
        ILogListener listener = decorator.Listener;

        if (listener.Interceptor is not null && listener.Interceptor.ShouldLog(httpRequest, listener) == false)
        {
            decorator.SkipHttpRequestIds.Add(httpRequest.Id);
            return;
        }

        listener.OnBeginRequest(httpRequest);
    }
}
