namespace SnapTraceV2.Models.Requests;

public class LogMessageRequest
{
    public string CategoryName { get; set; }
    public string LogLevel { get; set; }
    public string Message { get; set; }
    public double MillisecondsSinceStartRequest { get; set; }

    public string MemberType { get; set; }
    public string MemberName { get; set; }
    public int LineNumber { get; set; }
}
