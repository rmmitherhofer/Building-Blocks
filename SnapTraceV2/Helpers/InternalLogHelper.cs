using SnapTraceV2.Enums;

namespace SnapTraceV2.Helpers;

internal static class InternalLogHelper
{
    public static void LogException(Exception ex)
    {
        if (ex is null) return;

        Log(ex.ToString(), LogLevel.Error);
    }

    public static void Log(string message, LogLevel logLevel)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (SnapTraceOptionsConfiguration.InternalLog is null) return;

        string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        date = string.Format("{0,-22}", $"{date}");

        string log = logLevel.ToString();

        log = $"[{log}] ";

        message = $"'SnapTrace' {date}{log}{message}";

        try
        {
            SnapTraceOptionsConfiguration.InternalLog.Invoke(message);
        }
        catch { }
    }
}
