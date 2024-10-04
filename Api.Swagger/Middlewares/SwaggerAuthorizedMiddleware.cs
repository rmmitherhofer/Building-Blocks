using Microsoft.AspNetCore.Http;

namespace Swagger.Middlewares;

public class SwaggerAuthorizedMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerAuthorizedMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context) => await _next.Invoke(context);
}
