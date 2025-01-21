using SnapTraceV2.Helpers;
using SnapTraceV2.Models;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.Services;
using System.Net;

namespace SnapTraceV2.Args;

public static class FlushLogArgsFactory
{
    public static FlushLogArgs Create(LoggerService[] loggers)
    {
        if (loggers?.Any() != true) throw new ArgumentNullException(nameof(LoggerService));

        LoggerService defaultLogger = loggers.FirstOrDefault(p => p.Category == Constants.DefaultLoggerCategory) ?? loggers[0];

        var options = new FlushLogArgs.CreateOptions
        {
            MessagesGroups = GetLogMessages(loggers),
            Exceptions = GetExceptions(loggers),
            Files = GetFiles(loggers),
            CustomProperties = GetCustomProperties(loggers),
            IsCreatedByHttpRequest = defaultLogger.DataContainer.LoggerProperties.IsManagedByHttpRequest
        };

        HttpProperties httpProperties = defaultLogger.DataContainer.HttpProperties;

        httpProperties ??= new HttpProperties(new HttpRequest(new HttpRequest.CreateOptions
            {
                HttpMethod = "GET",
                Url = InternalHelper.GenerateUri(null),
                MachineName = InternalHelper.GetMachineName(),
                StartDateTime = defaultLogger.DataContainer.DateTimeCreated
            }));

        if (httpProperties.Response is null)
        {
            int statusCode = options.Exceptions.Count != 0 ? (int)HttpStatusCode.InternalServerError : (int)HttpStatusCode.OK;

            httpProperties.SetResponse(new HttpResponse(new HttpResponse.CreateOptions
            {
                StatusCode = statusCode
            }));
        }

        int? explicitStatusCode = GetExplicitStatusCode(loggers);
        if (explicitStatusCode.HasValue)
        {
            httpProperties.Response.SetStatusCode(explicitStatusCode.Value);
        }

        options.HttpProperties = httpProperties;

        return new FlushLogArgs(options);
    }

    private static List<LogMessagesGroup> GetLogMessages(LoggerService[] loggers)
    {
        List<LogMessagesGroup> result = [];

        foreach (LoggerService logger in loggers)
        {
            if (!logger.DataContainer.LogMessages.Any())
                continue;

            List<LogMessage> messages = logger.DataContainer.LogMessages.ToList();
            result.Add(new LogMessagesGroup(logger.Category, messages));
        }

        return result;
    }

    private static List<CapturedException> GetExceptions(LoggerService[] loggers)
    {
        List<CapturedException> result = [];

        foreach (LoggerService logger in loggers)
        {
            var exceptions = logger.DataContainer.Exceptions.Select(p => new CapturedException(p)).ToList();
            result.AddRange(exceptions);
        }

        return result;
    }

    private static List<LoggedFile> GetFiles(LoggerService[] loggers)
    {
        List<LoggedFile> result = [];

        foreach (LoggerService logger in loggers)
        {
            var files = logger.DataContainer.FilesContainer.GetLoggedFiles();
            result.AddRange(files);
        }

        return result;
    }

    private static List<KeyValuePair<string, object>> GetCustomProperties(LoggerService[] loggers)
    {
        List<KeyValuePair<string, object>> result = [];

        foreach (LoggerService logger in loggers)
        {
            result.AddRange(logger.DataContainer.LoggerProperties.CustomProperties);
        }

        return result;
    }

    internal static int? GetExplicitStatusCode(IEnumerable<LoggerService> loggers)
    {
        ArgumentNullException.ThrowIfNull(loggers, nameof(IEnumerable<LoggerService>));

        LoggerService? defaultLogger = loggers.FirstOrDefault(p => p.Category == Constants.DefaultLoggerCategory);

        int? result = defaultLogger?.DataContainer.LoggerProperties.ExplicitStatusCode;

        return result ?? (loggers.FirstOrDefault(p => p.DataContainer.LoggerProperties.ExplicitStatusCode.HasValue == true)?.DataContainer.LoggerProperties.ExplicitStatusCode);
    }
}
