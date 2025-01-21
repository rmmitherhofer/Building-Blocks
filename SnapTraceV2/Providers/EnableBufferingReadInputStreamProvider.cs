using Microsoft.AspNetCore.Http;
using System.Text;

namespace SnapTraceV2.Providers;

internal class EnableBufferingReadInputStreamProvider : IReadInputStreamProvider
{
    public const int BUFFER_SIZE = 1024;
    public string? ReadInputStream(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.Body.CanRead) return null;

        string? content = null;

        request.EnableBuffering();

        using (StreamReader reader = new(request.Body, Encoding.UTF8, true, BUFFER_SIZE, true))
        {
            var task = reader.ReadToEndAsync();
            task.Wait();

            content = task.Result;
        }

        request.Body.Position = 0;

        return content;
    }
}
