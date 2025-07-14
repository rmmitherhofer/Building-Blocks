using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Zypher.Extensions.Core;

namespace Zypher.Logs.Extensions;

/// <summary>
/// Provides extension methods to facilitate enriched logging with correlation IDs and color-coded console output.
/// Supports logging at different levels such as Debug, Trace, Information, Warning, Error, and Critical.
/// </summary>
public static class ConsoleLogExtensions
{
    private static IHttpContextAccessor? _accessor;

    /// <summary>
    /// Configures the console log extension with an IHttpContextAccessor instance to enable correlation ID retrieval.
    /// </summary>
    /// <param name="accessor">The HTTP context accessor.</param>
    public static void Configure(IHttpContextAccessor accessor) => _accessor = accessor;

    #region Dbug

    /// <summary>
    /// Logs a debug message including the correlation ID if available in the current HTTP context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The debug message.</param>
    /// <param name="args">Optional message format arguments.</param>
    public static void LogDbug(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogDebug($"{DateTime.Now.ToPtBrDateTime()}|DBUG|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogDebug($"{DateTime.Now.ToPtBrDateTime()}|DBUG|{message}", args);
    }

    /// <summary>
    /// Logs a debug message directly to the console with color formatting.
    /// </summary>
    /// <param name="message">The debug message.</param>
    /// <param name="args">Optional message format arguments.</param>
    public static void LogDbug(string message, params object?[] args) => CreateLog("DBUG", message, args);

    #endregion

    #region Trce

    /// <summary>
    /// Logs a trace message including the correlation ID if available in the current HTTP context.
    /// </summary>
    public static void LogTrce(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogTrace($"{DateTime.Now.ToPtBrDateTime()}|TRCE|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogTrace($"{DateTime.Now.ToPtBrDateTime()}|TRCE|{message}", args);
    }

    /// <summary>
    /// Logs a trace message directly to the console with color formatting.
    /// </summary>
    public static void LogTrce(string message, params object?[] args) => CreateLog("TRCE", message, args);

    #endregion

    #region Info

    /// <summary>
    /// Logs an informational message including the correlation ID if available in the current HTTP context.
    /// </summary>
    public static void LogInfo(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogInformation($"{DateTime.Now.ToPtBrDateTime()}|INFO|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogInformation($"{DateTime.Now.ToPtBrDateTime()}|INFO|{message}", args);
    }

    /// <summary>
    /// Logs an informational message directly to the console with color formatting.
    /// </summary>
    public static void LogInfo(string message, params object?[] args) => CreateLog("INFO", message, args);

    #endregion

    #region Warn

    /// <summary>
    /// Logs a warning message including the correlation ID if available in the current HTTP context.
    /// </summary>
    public static void LogWarn(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogWarning($"{DateTime.Now.ToPtBrDateTime()}|WARN|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogWarning($"{DateTime.Now.ToPtBrDateTime()}|WARN|{message}", args);
    }

    /// <summary>
    /// Logs a warning message directly to the console with color formatting.
    /// </summary>
    public static void LogWarn(string message, params object?[] args) => CreateLog("WARN", message, args);

    #endregion

    #region Fail

    /// <summary>
    /// Logs an error message including the correlation ID if available in the current HTTP context.
    /// </summary>
    public static void LogFail(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogError($"{DateTime.Now.ToPtBrDateTime()}|FAIL|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogError($"{DateTime.Now.ToPtBrDateTime()}|FAIL|{message}", args);
    }

    /// <summary>
    /// Logs an error message directly to the console with color formatting.
    /// </summary>
    public static void LogFail(string message, params object?[] args) => CreateLog("FAIL", message, args);

    #endregion

    #region Crit

    /// <summary>
    /// Logs a critical error message including the correlation ID if available in the current HTTP context.
    /// </summary>
    public static void LogCrit(this ILogger logger, string message, params object?[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_accessor?.HttpContext is not null && _accessor.HttpContext?.Request?.GetCorrelationId() is not null)
            logger.LogError($"{DateTime.Now.ToPtBrDateTime()}|CRIT|{_accessor.HttpContext?.Request?.GetCorrelationId()}|{message}", args);
        else
            logger.LogError($"{DateTime.Now.ToPtBrDateTime()}|CRIT|{message}", args);
    }

    /// <summary>
    /// Logs a critical error message directly to the console with color formatting.
    /// </summary>
    public static void LogCrit(string message, params object?[] args) => CreateLog("CRIT", message, args);

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
        Console.Write($": {DateTime.Now.ToPtBrDateTime()}|");
        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.Write(logLevel.ToUpper());
        Console.ResetColor();
        Console.Write($"|");

        if (_accessor?.HttpContext is not null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{_accessor.HttpContext.Request?.GetCorrelationId()}");
            Console.ResetColor();
            Console.Write($"|");
        }

        Console.Write($"{string.Format(message, args)}");
        Console.WriteLine();
    }
}
