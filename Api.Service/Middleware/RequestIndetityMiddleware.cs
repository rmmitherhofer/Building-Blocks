using Common.Core.Users;
using Microsoft.AspNetCore.Http;

namespace Api.Service.Middleware;

public class RequestIndetityMiddleware
{
    public const string Name = "RequestIndetityMiddleware";
    private readonly RequestDelegate _next;

    public RequestIndetityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        UserRequest user = new(context);

        await _next(context);
    }
}