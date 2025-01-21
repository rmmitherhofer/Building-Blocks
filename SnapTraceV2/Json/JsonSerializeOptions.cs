namespace SnapTraceV2.Json;

public class JsonSerializeOptions
{
    public bool WriteIndented { get; set; }

    public JsonSerializeOptions() => WriteIndented = true;
}
