using Microsoft.Extensions.Logging;

namespace SnapTrace.Models;

public class Log
{
    public Project Project { get; set; }
    public string Path { get; set; }
    public string? RequisitionTime { get; set; }
    public LogLevel LogLevel { get; set; }
    public User User { get; set; }
    public Request Request { get; set; }
    public Response Response { get; set; }
    public string ErrorType { get; set; }
    public IEnumerable<LogEntry> Entries { get; set; }
    public IEnumerable<Error> Errors { get; set; }

}
