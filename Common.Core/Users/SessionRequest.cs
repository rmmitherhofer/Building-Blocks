namespace Common.Core.Users;

public class SessionRequest
{
    public string RequestId { get; set; }
    public string CorrelationId { get; set; }
    public string ClientId { get; set; }
    public string Method { get; set; }
    public string Url { get; set; }
    public string Referer { get; set; }
}
