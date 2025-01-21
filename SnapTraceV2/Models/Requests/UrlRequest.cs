namespace SnapTraceV2.Models.Requests;

public class UrlRequest
{
    public string Path { get; set; }
    public string PathAndQuery { get; set; }
    public string AbsoluteUri { get; set; }
}
