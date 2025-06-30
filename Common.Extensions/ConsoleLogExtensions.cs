using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Extensions;

public static class ConsoleLogExtensions
{
    private static IHttpContextAccessor? _accessor;

    public static void Configure(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    #region Dbug

    public static void LogDbug(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogDebug($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogDebug($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG|{message}", args);
    }
    public static void LogDbug(string message, params object?[] args) => CreateLog("DBUG", message);

    #endregion

    #region Trce
    public static void LogTrce(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogTrace($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogTrace($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE|{message}", args);
    }
    public static void LogTrce(string message, params object?[] args) => CreateLog("TRCE", message);
    #endregion

    #region Info
    public static void LogInfo(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogInformation($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogInformation($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO|{message}", args);
    }
    public static void LogInfo(string message, params object?[] args) => CreateLog("INFO", message);
    #endregion

    #region Warn
    public static void LogWarn(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogWarning($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogWarning($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN|{message}", args);
    }
    public static void LogWarn(string message, params object?[] args) => CreateLog("WARN", message);
    #endregion

    #region Fail
    public static void LogFail(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL|{message}", args);
    }

    public static void LogFail(string message, params object?[] args) => CreateLog("FAIL", message);

    #endregion

    #region Crit
    public static void LogCrit(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.GetCorrelationId() is not null)
            logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT|{_accessor.HttpContext?.GetCorrelationId()}|{message}", args);
        else
            logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT|{message}", args);
    }
    public static void LogCrit(string message, params object?[] args) => CreateLog("CRIT", message);
    #endregion
    private static void CreateLog(string logLevel, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        switch (logLevel.ToUpper())
        {
            case "DBUG":
            case "TRCE":
                MessageDecorator(logLevel, message, ConsoleColor.White, ConsoleColor.Black, args);
                break;
            case "INFO":
                MessageDecorator(logLevel, message, ConsoleColor.DarkGreen, ConsoleColor.Black, args);
                break;
            case "WARN":
                MessageDecorator(logLevel, message, ConsoleColor.Yellow, ConsoleColor.Black, args);
                break;
            case "FAIL":
            case "CRIT":
                MessageDecorator(logLevel, message, ConsoleColor.Black, ConsoleColor.DarkRed, args);
                break;
        }
    }

    private static void MessageDecorator(string logLevel, string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor = ConsoleColor.Black, params object?[] args)
    {
        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.Write(logLevel.ToLower());
        Console.ResetColor();
        Console.Write($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|");
        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.Write(logLevel.ToUpper());
        Console.ResetColor();
        Console.Write($"|");

        if (_accessor?.HttpContext is not null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{_accessor.HttpContext.GetCorrelationId()}");
            Console.ResetColor();
            Console.Write($"|");
        }

        Console.Write($"{string.Format(message, args)}");
        Console.WriteLine();
    }
}

