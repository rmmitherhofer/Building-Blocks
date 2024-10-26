using Microsoft.AspNetCore.Http;

namespace SnapTrace.Middleware;

public class BodyBufferingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        await _next(context);
    }
}
