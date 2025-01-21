using SnapTraceV2.Services;

namespace SnapTraceV2.Factories;

internal class LogResponseBody : ILogResponseBodyStrategy
{
    private readonly LoggerService _logger;
    private readonly string _responseFileName;
    private readonly IReadStreamStrategy _readStreamStrategy;
    public LogResponseBody(
        LoggerService logger,
        string responseFileName,
        IReadStreamStrategy readStreamStrategy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(responseFileName, nameof(responseFileName));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _readStreamStrategy = readStreamStrategy ?? throw new ArgumentNullException(nameof(readStreamStrategy));
        _responseFileName = responseFileName;
    }

    public void Execute()
    {
        ReadStreamResult result = _readStreamStrategy.Read();

        //if (result.TemporaryFile != null)
        //{
        //    _logger.DataContainer.FilesContainer.LogFile(result.TemporaryFile.FileName, _responseFileName);
        //}
        //else if (!string.IsNullOrEmpty(result.Content))
        //{
        //    _logger.DataContainer.FilesContainer.LogAsFile(result.Content, _responseFileName);
        //}
    }
}
