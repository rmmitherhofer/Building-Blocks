namespace SnapTraceV2.Models.Http;

public class HttpProperties
{
    public HttpRequest Request { get; }
    public HttpResponse? Response { get; private set; }

    internal HttpProperties(HttpRequest httpRequest) => Request = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));

    internal void SetResponse(HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponse));

        Response = response;
    }

    internal HttpProperties Clone()
    {
        HttpRequest request = Request.Clone();

        HttpResponse? response = Response?.Clone();

        return new(request)
        {
            Response = response
        };
    }
}