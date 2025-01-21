namespace SnapTraceV2.Models.Requests;

public class CreateRequestLogRequest
{
    //REQUEST QUE É ENVIADO PARA API
    public string SdkName { get; set; }
    public string SdkVersion { get; set; }

    public string ClientId { get; set; }
    public string OrganizationId { get; set; }
    public string ApplicationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public double DurationInMilliseconds { get; set; }
    public bool IsNewSession { get; set; }
    public string SessionId { get; set; }
    public string MachineName { get; set; }
    public bool IsAuthenticated { get; set; }
    public UserRequest User { get; set; }
    public HttpPropertiesRequest WebRequest { get; set; }
    public IEnumerable<LogMessageRequest> LogMessages { get; set; }
    public IEnumerable<CapturedExceptionRequest> Exceptions { get; set; }
    public IEnumerable<string> Keywords { get; set; }
    public List<KeyValuePair<string, object>> CustomProperties { get; set; }

    public CreateRequestLogRequest()
    {
        ClientId = Guid.NewGuid().ToString();
        LogMessages = [];
        Exceptions = [];
        Keywords = [];
        CustomProperties = [];
    }
}
