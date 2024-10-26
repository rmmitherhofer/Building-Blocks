namespace SnapTrace.Models;

public class Validation
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}