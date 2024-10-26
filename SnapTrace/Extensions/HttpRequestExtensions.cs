using Microsoft.AspNetCore.Http;

namespace SnapTrace.Extensions;

public static class HttpRequestExtensions
{
    public static string GetBody(this HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);

        string body;

        using (StreamReader reader = new(request.Body))
        {
            body = reader.ReadToEnd();
        }

        return body ?? string.Empty;
    }
}
