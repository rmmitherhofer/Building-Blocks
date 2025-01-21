namespace SnapTraceV2.Models.Requests;

public class ResponsePropertiesRequest
{
    public string HttpStatusCodeText { get; set; }

    public int HttpStatusCode { get; set; }

    public List<KeyValuePair<string, string?>> Headers { get; set; }

    public long ContentLength { get; set; }

    public ResponsePropertiesRequest() => Headers = [];
}
