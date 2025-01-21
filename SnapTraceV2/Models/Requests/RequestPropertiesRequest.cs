namespace SnapTraceV2.Models.Requests;

public class RequestPropertiesRequest
{
    public List<KeyValuePair<string, string?>> Headers { get; set; }
    public List<KeyValuePair<string, string?>> Cookies { get; set; }
    public List<KeyValuePair<string, string?>> QueryString { get; set; }
    public List<KeyValuePair<string, string?>> FormData { get; set; }
    public List<KeyValuePair<string, string?>> ServerVariables { get; set; }
    public List<KeyValuePair<string, string?>> Claims { get; set; }
    public string InputStream { get; set; }

    public RequestPropertiesRequest()
    {
        Headers = [];
        Cookies = [];
        QueryString = [];
        FormData = [];
        ServerVariables = [];
        Claims = [];
    }
}
