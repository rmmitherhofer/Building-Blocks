namespace SnapTraceV2.Models;

public class CapturedException
{
    public string Type { get; }
    public string Message { get; }
    public string ExceptionString { get; }

    public CapturedException(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        Type = ex.GetType().FullName;
        Message = ex.Message;
        ExceptionString = ex.ToString();
    }
}
