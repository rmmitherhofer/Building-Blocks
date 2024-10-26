namespace SnapTrace.Models;

public class Error
{
    public string Type { get; set; }
    public string Message { get; set; }
    public string? Tracer { get; set; }
    public string? Datail { get; set; }
}
