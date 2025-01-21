using SnapTraceV2.Models;

namespace SnapTraceV2.Args;

public class ExceptionArgs
{
    public FlushLogArgs FlushArgs { get; }
    public ApiResult ApiResult { get; }

    public ExceptionArgs(FlushLogArgs flushArgs, ApiResult apiResult)
    {
        ArgumentNullException.ThrowIfNull(flushArgs, nameof(FlushLogArgs));
        ArgumentNullException.ThrowIfNull(apiResult, nameof(ApiResult));

        FlushArgs = flushArgs;
        ApiResult = apiResult;
    }
}
