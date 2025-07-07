using Microsoft.AspNetCore.Http;

namespace Api.Swagger.Middlewares;

/// <summary>
/// Middleware to restrict access to Swagger UI endpoints.
/// </summary>
/// <remarks>
/// By default, this middleware allows all requests.
/// You can enable protection by uncommenting and customizing the logic
/// inside the <see cref="Invoke"/> method, for example,
/// to allow access only for authenticated users.
/// </remarks>
public class SwaggerAuthorizedMiddleware
{
    /// <summary>
    /// The name identifier of this middleware.
    /// </summary>
    public const string Name = "SwaggerAuthorizedMiddleware";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="SwaggerAuthorizedMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    public SwaggerAuthorizedMiddleware(RequestDelegate next) => _next = next;

    /// <summary>
    /// Processes the HTTP request and optionally restricts access to Swagger UI endpoints.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        // Uncomment the following code to restrict access to Swagger endpoints:

        /*
        // Apply protection only if the request is for Swagger UI
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            // Example: check if user is authenticated
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized access to Swagger UI.");
                return; // block access
            }
        }
        */

        await _next(context);
    }
}
