using SnapTraceV2.Decorators;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.NotifyListeners;

internal static class NotifyOnMessage
{
    public static void Notify(LogMessage message, Guid? httpRequestId = null)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(LogMessage));

        foreach (LogListenerDecorator decorator in SnapTraceOptionsConfiguration.Listeners.GetAll())
            InternalHelper.WrapInTryCatch(() => Notify(message, decorator, httpRequestId));
    }

    private static void Notify(LogMessage message, LogListenerDecorator decorator, Guid? httpRequestId = null)
    {
        if (httpRequestId != null && decorator.SkipHttpRequestIds.Contains(httpRequestId.Value)) return;

        ILogListener listener = decorator.Listener;

        if (listener.Interceptor?.ShouldLog(message, listener) == false) return;

        listener.OnMessage(message);
    }
}
