using SnapTraceV2.Args;

namespace SnapTraceV2.Options;

internal class FlushOptions
{
    public bool UseAsync { get; set; }
    public Action<ExceptionArgs> OnException { get; set; }
}
