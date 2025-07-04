using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SnapTrace.Configurations.Settings;
using SnapTrace.Extensions;
using System.Text.Json;

namespace SnapTrace.Middleware;

public class CaptureResponseBodyMiddleware(RequestDelegate next, SensitiveDataMasker sensitiveDataMasker, IOptions<SnapTraceSettings> options)
{
    public const string Name = "CaptureResponseBodyMiddleware";
    private readonly SnapTraceSettings _settings = options.Value;
    private readonly RequestDelegate _next = next;
    private readonly SensitiveDataMasker _sensitiveDataMasker = sensitiveDataMasker;

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        string capturedBody;

        long maxSize = _settings.MaxResponseBodySizeInMb * 1024L * 1024L;

        if (memoryStream.Length <= maxSize)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            capturedBody = await new StreamReader(memoryStream).ReadToEndAsync();
        }
        else
        {
            capturedBody = $"[Body not captured. Size exceeds limit of {_settings.MaxResponseBodySizeInMb}MB]";
        }

        context.Items["CapturedResponseBody"] = capturedBody;

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
