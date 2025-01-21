using SnapTraceV2.Exceptions;
using SnapTraceV2.Services;

namespace SnapTraceV2.Factories;

internal class LogResponseBodySizeTooLargeException : ILogResponseBodyStrategy
{
    private readonly LoggerService _logger;
    private readonly long _contentLength;
    private readonly long _maximumAllowedFileSizeInBytes;
    public LogResponseBodySizeTooLargeException(LoggerService logger, long contentLength, long maximumAllowedFileSizeInBytes)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (contentLength < 0)
            throw new ArgumentException(nameof(contentLength));

        if (contentLength <= maximumAllowedFileSizeInBytes)
            throw new ArgumentException(nameof(maximumAllowedFileSizeInBytes));

        _logger = logger;
        _contentLength = contentLength;
        _maximumAllowedFileSizeInBytes = maximumAllowedFileSizeInBytes;
    }

    public void Execute()
    {
        //_logger.Warn(new ResponseBodySizeTooLargeException(_contentLength, _maximumAllowedFileSizeInBytes));
    }
}
