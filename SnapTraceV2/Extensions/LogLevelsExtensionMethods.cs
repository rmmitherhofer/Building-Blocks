using SnapTraceV2.Enums;
using SnapTraceV2.Json;
using SnapTraceV2.Services;

namespace SnapTraceV2.Extensions;

public static class LogLevelsExtensions
{
    #region Trace

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Trace(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Trace, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Trace(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Trace, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Trace(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Trace, ex, memberName, lineNumber, memberType);
    }

    public static void Trace(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Trace, args, options, memberName, lineNumber, memberType);
    }

    #endregion

    #region Debug

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Debug(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Debug, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Debug(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Debug, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Debug(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Debug, ex, memberName, lineNumber, memberType);
    }
    public static void Debug(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Debug, args, options, memberName, lineNumber, memberType);
    }

    #endregion

    #region Info

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Info(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Information, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Info(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Information, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Info(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Information, ex, memberName, lineNumber, memberType);
    }

    public static void Info(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Information, args, options, memberName, lineNumber, memberType);
    }

    #endregion

    #region Warn

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Warn(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Warning, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Warn(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Warning, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Warn(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Warning, ex, memberName, lineNumber, memberType);
    }

    public static void Warn(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Warning, args, options, memberName, lineNumber, memberType);
    }

    #endregion

    #region Error

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Error(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Error, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Error(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Error, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Error(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Error, ex, memberName, lineNumber, memberType);
    }

    public static void Error(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Error, args, options, memberName, lineNumber, memberType);
    }

    #endregion

    #region Critical

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="message">Log data</param>
    public static void Critical(this ILoggerService logger, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Critical, message, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="json">Log data</param>
    public static void Critical(this ILoggerService logger, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Critical, json, options, memberName, lineNumber, memberType);
    }

    /// <summary>
    /// Writes a Log message
    /// </summary>
    /// <param name="ex">Log data</param>
    public static void Critical(this ILoggerService logger, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Critical, ex, memberName, lineNumber, memberType);
    }

    public static void Critical(this ILoggerService logger, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null)
    {
        if (logger == null)
            return;

        logger.Log(LogLevel.Critical, args, options, memberName, lineNumber, memberType);
    }

    #endregion
}
