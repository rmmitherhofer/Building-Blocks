using SnapTraceV2.Containeres;
using SnapTraceV2.Options;

namespace SnapTraceV2.Builders;

public interface IOptionsBuilder
{
    LogListenersContainer Listeners { get; }
    HandlerOptions Options { get; }
    Action<string> InternalLog { get; set; }
}
public class OptionsBuilder : IOptionsBuilder
{
    public LogListenersContainer Listeners => SnapTraceOptionsConfiguration.Listeners;
    public HandlerOptions Options => SnapTraceOptionsConfiguration.Options;
    public Action<string> InternalLog
    {
        get
        {
            return SnapTraceOptionsConfiguration.InternalLog;
        }
        set
        {
            SnapTraceOptionsConfiguration.InternalLog = value;
        }
    }
}
