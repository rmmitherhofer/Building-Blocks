using SnapTraceV2.Decorators;
using SnapTraceV2.NotifyListeners;

namespace SnapTraceV2.Containeres;

public class LogListenersContainer
{
    private readonly List<LogListenerDecorator> _listeners;
    internal LogListenersContainer() => _listeners = [];

    public LogListenersContainer Add(ILogListener listener)
    {
        if (listener is null) return this;

        _listeners.Add(new LogListenerDecorator(listener));

        return this;
    }

    internal List<LogListenerDecorator> GetAll() 
        => _listeners.ConvertAll(p => p);
}
