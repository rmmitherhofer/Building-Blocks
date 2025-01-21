using SnapTraceV2.Services;

namespace SnapTraceV2.Args;

public class FormatterArgs
{
    public object State { get; }
    public Exception Exception { get; }
    public string DefaultValue { get; }
    public LoggerService Logger { get; }

    internal FormatterArgs(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));
        ArgumentNullException.ThrowIfNull(options.Logger, nameof(LoggerService));

        State = options.State;
        Exception = options.Exception;
        DefaultValue = options.DefaultValue;
        Logger = options.Logger;
    }

    internal class CreateOptions
    {
        public object State { get; set; }
        public Exception Exception { get; set; }
        public string DefaultValue { get; set; }
        public LoggerService Logger { get; set; }
    }
}
