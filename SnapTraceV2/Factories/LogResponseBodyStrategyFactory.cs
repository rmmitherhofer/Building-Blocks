using SnapTraceV2.Helpers;
using SnapTraceV2.Services;
using System.Text;

namespace SnapTraceV2.Factories;

internal class LogResponseBodyStrategyFactory
{
    public static ILogResponseBodyStrategy Create(Stream stream, Encoding encoding, LoggerService logger)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(Stream));

        ArgumentNullException.ThrowIfNull(encoding, nameof(Encoding));

        ArgumentNullException.ThrowIfNull(logger, nameof(LoggerService));

        if (!stream.CanRead) return new NullLogResponseBody();

        if (stream.Length > Constants.MaximumAllowedFileSizeInBytes)
            return new LogResponseBodySizeTooLargeException(logger, stream.Length, Constants.MaximumAllowedFileSizeInBytes);

        var headers = logger.DataContainer.HttpProperties.Response?.Properties?.Headers;

        string contentType = headers?.FirstOrDefault(p => string.Compare(p.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value;

        string responseFileName = InternalHelper.GenerateResponseFileName(headers);

        IReadStreamStrategy strategy = ReadStreamStrategyFactory.Create(stream, encoding, contentType);
        return new LogResponseBody(logger, responseFileName, strategy);
    }
}
