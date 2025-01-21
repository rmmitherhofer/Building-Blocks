namespace SnapTraceV2.Models.Requests;

public class CapturedExceptionRequest
{
    public string ExceptionType { get; set; }
    public string ExceptionMessage { get; set; }
}
