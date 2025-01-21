using SnapTraceV2.Enums;
using SnapTraceV2.Factories;
using SnapTraceV2.Helpers;
using SnapTraceV2.Json;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.NotifyListeners;

namespace SnapTraceV2.Services;

public class LoggerService : ILoggerService
{
    internal Guid Id { get; }
    public LoggerDataContainer DataContainer { get; private set; }
    public static ILoggerFactory Factory { get; internal set; } = new LoggerFactory();

    public string Category { get; }
    public LoggerService(string? category = null, string? url = null)
    {
        Id = Guid.NewGuid();

        Category = string.IsNullOrWhiteSpace(category) ? Constants.DefaultLoggerCategory : category;

        DataContainer = new(this);

        HttpProperties? httpProperties = string.IsNullOrWhiteSpace(url) ? null : HttpPropertiesFactory.Create(url);

        if (httpProperties is not null)
        {
            DataContainer.SetHttpProperties(httpProperties);

            InternalHelper.WrapInTryCatch(() => NotifyBeginRequest.Notify(httpProperties.Request));
        }
    }

    internal void Reset()
    {
        HttpProperties httpProperties = DataContainer.HttpProperties;

        DataContainer.Dispose();
        DataContainer = new LoggerDataContainer(this);

        if (httpProperties is not null)
            DataContainer.SetHttpProperties(httpProperties);
    }

    public void Log(LogLevel logLevel, string message, string? memberName = null, int lineNumber = 0, string? memberType = null)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        LogMessage logMessage = new(new LogMessage.CreateOptions
        {
            Category = Category,
            LogLevel = logLevel,
            Message = message,
            MemberType = memberType,
            MemberName = memberName,
            LineNumber = lineNumber
        });

        DataContainer.Add(logMessage);

        Guid? httpRequestId = DataContainer.HttpProperties?.Request.Id;

        InternalHelper.WrapInTryCatch(() => NotifyOnMessage.Notify(logMessage, httpRequestId));
    }

    public void Log(LogLevel logLevel, object json, JsonSerializeOptions? options = null, string? memberName = null, int lineNumber = 0, string? memberType = null)
    {
        string message = SnapTraceOptionsConfiguration.JsonSerializer.Serialize(json, options);
        Log(logLevel, message, memberName, lineNumber, memberType);
    }

    public void Log(LogLevel logLevel, Exception ex, string? memberName = null, int lineNumber = 0, string? memberType = null)
    {
        if (ex is null) return;

        var formatter = new ExceptionFormatter();
        string message = formatter.Format(ex, this);

        Log(logLevel, message, memberName, lineNumber, memberType);
    }

    public void Log(LogLevel logLevel, Args.Args args, JsonSerializeOptions? options = null, string? memberName = null, int lineNumber = 0, string? memberType = null)
    {
        if (args is null) return;

        List<string> values = [];

        foreach (var arg in args.GetArgs())
        {
            string? message = null;

            if (arg is string stringArg)
            {
                message = stringArg;
            }
            else if (arg is Exception exceptionArg)
            {
                var formatter = new ExceptionFormatter();
                message = formatter.Format(exceptionArg, this);
            }
            else
            {
                message = SnapTraceOptionsConfiguration.JsonSerializer.Serialize(arg, options);
            }

            if (string.IsNullOrEmpty(message)) continue;

            values.Add(message);
        }

        string value = string.Join(Environment.NewLine, values);

        Log(logLevel, value, memberName, lineNumber, memberType);
    }
}
