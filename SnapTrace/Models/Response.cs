using Microsoft.Extensions.Primitives;
using System.Net;

namespace SnapTrace.Models;

public class Response
{
    public HttpStatusCode StatusCode { get; set; }
    public IDictionary<string, StringValues>? Headers { get; set; }
    public double BodySize { get; set; }
}
