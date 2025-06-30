using Common.Notifications.Messages;
using Microsoft.Extensions.Logging;

namespace SnapTrace.Models;

public class LogEntry
{
    public string Category { get; private set; }
    public LogLevel LogLevel { get; private set; }
    public string Message { get; set; }
    public string? MemberType { get; set; }
    public string? MemberName { get; set; }
    public int LineNumber { get; set; }
    public DateTime DateTime { get; private set; }

    public LogEntry(string category, LogLevel logLevel, string message)
    {
        Category = category;
        LogLevel= logLevel;
        Message = message;
        DateTime = DateTime.UtcNow;
    }
}