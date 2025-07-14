using Microsoft.AspNetCore.Http;
using Zypher.Domain.Core.Users;

namespace Zypher.Api.Foundation.Middleware;

/// <summary>
/// Middleware to capture and store the user identity from the current HttpContext.
/// This allows user information to be accessed during the request pipeline.
/// </summary>
public class RequestIndetityMiddleware
{
    public const string Name = nameof(RequestIndetityMiddleware);
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor that accepts the next middleware in the pipeline.
    /// </summary>
    /// <param name="next">The next RequestDelegate.</param>
    public RequestIndetityMiddleware(RequestDelegate next) => _next = next;

    /// <summary>
    /// Invokes the middleware to process the current HttpContext and capture the user identity.
    /// </summary>
    /// <param name="context">The current HttpContext.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        UserRequest user = new(context);

        await _next(context);
    }
}
