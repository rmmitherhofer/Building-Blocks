using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Common.Extensions;

namespace Logs.Extensions;

public static class ConsoleLog
{
    #region Dbug
    public static void LogDbug(this ILogger logger, string message)
        => logger.LogDbug($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG| {message}");
    public static void LogDbug(this ILogger logger, string message, HttpContext? context)
        => logger.LogDbug($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG|{context?.GetSessionId()}: {message}");
    public static void LogDbug(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.Write($"dbug: {DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG| {message}");
        Console.WriteLine();
    }
    public static void LogDbug(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.Write($"dbug: {DateTime.Now:dd/MM/yyyy HH:mm:ss}|DBUG|{context?.GetSessionId()}: {message}");
        Console.WriteLine();
    }

    #endregion

    #region Trce
    public static void LogTrce(this ILogger logger, string message)
        => logger.LogTrace($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE| {message}");
    public static void LogTrce(this ILogger logger, string message, HttpContext? context)
        => logger.LogTrace($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE|{context?.GetSessionId()}: {message}");
    public static void LogTrce(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.Write($"trce: {DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE| {message}");
        Console.WriteLine();
    }
    public static void LogTrce(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.Write($"trce: {DateTime.Now:dd/MM/yyyy HH:mm:ss}|TRCE|{context?.GetSessionId()}: {message}");
        Console.WriteLine();
    }

    #endregion

    #region Info
    public static void LogInfo(this ILogger logger, string message)
        => logger.LogInformation($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO| {message}");
    public static void LogInfo(this ILogger logger, string message, HttpContext? context)
        => logger.LogInformation($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO|{context?.GetSessionId()}: {message}");
    public static void LogInfo(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("info");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO| {message}");
    }
    public static void LogInfo(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("info");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|INFO|{context?.GetSessionId()}: {message}");
    }
    #endregion

    #region Warn
    public static void LogWarn(this ILogger logger, string message) 
        => logger.LogWarning($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN| {message}");
    public static void LogWarn(this ILogger logger, string message, HttpContext? context)
        => logger.LogWarning($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN|{context?.GetSessionId()}: {message}");
    public static void LogWarn(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("warn");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN| {message}");
    }
    public static void LogWarn(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("warn");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|WARN|{context?.GetSessionId()}: {message}");
    }
    #endregion

    #region Fail
    public static void LogFail(this ILogger logger, string message) 
        => logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL| {message}");
    public static void LogFail(this ILogger logger, string message, HttpContext? context) 
        => logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL|{context?.GetSessionId()}: {message}");
    public static void LogFail(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("fail");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL| {message}");
    }
    public static void LogFail(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("fail");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|FAIL|{context?.GetSessionId()}: {message}");
    }
    #endregion

    #region Crit
    public static void LogCrit(this ILogger logger, string message) 
        => logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT| {message}");
    public static void LogCrit(this ILogger logger, string message, HttpContext? context) 
        => logger.LogError($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT|{context?.GetSessionId()}: {message}");
    public static void LogCrit(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("crit");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT| {message}");
    }
    public static void LogCrit(string message, HttpContext? context)
    {
        if (string.IsNullOrEmpty(message)) return;

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("crit");
        Console.ResetColor();
        Console.WriteLine($": {DateTime.Now:dd/MM/yyyy HH:mm:ss}|CRIT|{context?.GetSessionId()}: {message}");
    }
    #endregion
}
