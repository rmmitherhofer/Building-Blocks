namespace SnapTraceV2.Models.Requests;

public class HttpPropertiesRequest
{
    public UrlRequest Url { get; set; }

    public string UserAgent { get; set; }

    public string HttpMethod { get; set; }

    public string RemoteAddress { get; set; }

    public string HttpReferer { get; set; }

    public RequestPropertiesRequest Request { get; set; }

    public ResponsePropertiesRequest Response { get; set; }
}
