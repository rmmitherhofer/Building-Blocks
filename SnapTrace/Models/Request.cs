using Microsoft.Extensions.Primitives;

namespace SnapTrace.Models;

public class Request
{
    public string Id { get; set; }
    public string Method { get; set; }
    public string Url { get; set; }
    public string? UserAgent { get; set; }
    public string CorrelationId { get; set; }
    public string? ClientId { get; set; }
    public IDictionary<string, StringValues>? Headers { get; set; }
    public IDictionary<string, StringValues>? Body { get; set; }
    public double BodySize { get; set; }
}
