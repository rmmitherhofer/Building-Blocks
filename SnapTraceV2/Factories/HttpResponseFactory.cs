using SnapTraceV2.Extensions;
using SnapTraceV2.Models.Http;

namespace SnapTraceV2.Factories;

internal static class HttpResponseFactory
{
    public static HttpResponse Create(Microsoft.AspNetCore.Http.HttpResponse httpResponse, long contentLength)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (contentLength < 0)
            throw new ArgumentException(nameof(contentLength));

        var options = new HttpResponse.CreateOptions
        {
            StatusCode = httpResponse.StatusCode,
            Properties = new ResponseProperties(new ResponseProperties.CreateOptions
            {
                Headers = httpResponse.Headers.ToKeyValuePair(),
                ContentLength = contentLength
            })
        };

        return new HttpResponse(options);
    }
}
