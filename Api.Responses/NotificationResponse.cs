using Microsoft.Extensions.Logging;

namespace Api.Responses;

public class NotificationResponse : MessageResponse
{
    public Guid Id { get;  set; }
    public DateTime Timestamp { get;  set; }
    public LogLevel Level { get;  set; }
    public string? Key { get;  set; }
    public string Value { get;  set; }

    public string Detail { get;  set; }    
}

public abstract class MessageResponse
{
    public string Type { get; set; }
    public Guid AggregateId { get; set; }
}
