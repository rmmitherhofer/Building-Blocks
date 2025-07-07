using Api.Swagger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Api.Swagger.Middlewares;

/// <summary>
/// Middleware to validate the presence of required HTTP headers in incoming requests.
/// Returns HTTP 400 Bad Request if any required header is missing.
/// </summary>
public class RequestValidateHeadersMiddleware
{
    /// <summary>
    /// The name identifier of this middleware.
    /// </summary>
    public const string Name = "RequestValidateHeadersMiddleware";

    private readonly RequestDelegate _next;
    private readonly SwaggerDefaultHeadersOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestValidateHeadersMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="options">The configuration options for default headers validation.</param>
    public RequestValidateHeadersMiddleware(RequestDelegate next, IOptions<SwaggerDefaultHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    /// <summary>
    /// Invokes the middleware to validate required headers in the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var missingHeaders = new List<string>();

        void CheckHeader(string headerName, bool required, bool enabled)
        {
            if (enabled && required && !context.Request.Headers.ContainsKey(headerName))
            {
                missingHeaders.Add(headerName);
            }
        }

        CheckHeader("X-Client-ID", _options.XClientId.Required, _options.XClientId.Enabled);
        CheckHeader("X-Forwarded-For", _options.XForwardedFor.Required, _options.XForwardedFor.Enabled);
        CheckHeader("X-Correlation-ID", _options.XCorrelationId.Required, _options.XCorrelationId.Enabled);
        CheckHeader("User-Agent", _options.UserAgent.Required, _options.UserAgent.Enabled);

        foreach (var header in _options.AdditionalHeaders)
        {
            if (!string.IsNullOrWhiteSpace(header.Name))
            {
                CheckHeader(header.Name, header.Required, true);
            }
        }

        if (missingHeaders.Count > 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                Message = "Missing required headers.",
                MissingHeaders = missingHeaders
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        await _next(context);
    }
}
