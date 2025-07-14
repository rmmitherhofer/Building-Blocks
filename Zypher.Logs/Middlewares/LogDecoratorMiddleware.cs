using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text;
using Zypher.Extensions.Core;
using Zypher.Logs.Extensions;
using Zypher.User.Extensions;

namespace Zypher.Logs.Middlewares;

/// <summary>
/// Middleware that decorates HTTP requests and responses with detailed logging information.
/// Logs request and response details including timing, status codes, and correlation identifiers.
/// </summary>
internal class LogDecoratorMiddleware
{
    /// <summary>
    /// Middleware identifier name.
    /// </summary>
    public const string Name = nameof(LogDecoratorMiddleware);

    private readonly ILogger<LogDecoratorMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogDecoratorMiddleware"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging information.</param>
    /// <param name="next">Next middleware delegate in the pipeline.</param>
    public LogDecoratorMiddleware(ILogger<LogDecoratorMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP context asynchronously.
    /// Logs request start, response details, timing, and handles exceptions.
    /// </summary>
    /// <param name="context">HTTP context of the current request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch diagnostic = new();
        diagnostic.Start();

        HeaderRequest(context);
        _logger.LogInfo($"Request:{context.Request.GetFullUrl()}");

        await _next(context);

        diagnostic.Stop();

        _logger.LogInfo(
            $"Response:{context.Request.GetFullUrl()} - Timer request: {diagnostic.GetFormattedTime()} - " +
            $"Status code: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}");
    }

    /// <summary>
    /// Logs HTTP request headers and contextual information to the console.
    /// </summary>
    /// <param name="context">HTTP context of the current request.</param>
    private void HeaderRequest(HttpContext context)
    {
        StringBuilder builder = new();

        builder.AppendLine("====================================================================================================");
        builder.AppendLine($"Date/Hour             : {DateTime.Now.ToPtBrDateTime()}");
        builder.AppendLine($"Method                : {context.Request.Method}");
        builder.AppendLine($"Url                   : {context.Request.GetFullUrl()}");
        builder.AppendLine($"Request-ID            : {context.Request.GetRequestId()}");
        builder.AppendLine($"Correlation-ID        : {context.Request.GetCorrelationId()}");

        if (!string.IsNullOrEmpty(context.Request.GetClientId()))
            builder.AppendLine($"Client-ID             : {context.Request.GetClientId()}");

        builder.AppendLine($"IsAjaxRequest         : {context.Request.IsAjaxRequest()}");

        if (!string.IsNullOrEmpty(context.Request.GetUserAgent()))
            builder.AppendLine($"User-Agent            : {context.Request.GetUserAgent()}");

        if (!string.IsNullOrEmpty(context.Request.GetIpAddress()))
            builder.AppendLine($"IP-Address            : {context.Request.GetIpAddress()}");

        if (!string.IsNullOrEmpty(context.Request.GetPodName()))
            builder.AppendLine($"Pod-Request           : {context.Request.GetPodName()}");

        if (context.User.IsAuthenticated())
        {
            builder.AppendLine($"IsAuthenticated       : {true}");

            if (!string.IsNullOrEmpty(context.User.GetId()))
                builder.AppendLine($"User-ID           : {context.Request.GetUserId()}");

            if (!string.IsNullOrEmpty(context.User.GetAccountCode()))
                builder.AppendLine($"User-Account-Code : {context.User.GetAccountCode()}");

            if (!string.IsNullOrEmpty(context.User.GetAccount()))
                builder.AppendLine($"User-Account      : {context.User.GetAccount()}");

            if (!string.IsNullOrEmpty(context.User.GetName()))
                builder.AppendLine($"User-Name         : {context.User.GetName()}");

            if (!string.IsNullOrEmpty(context.User.GetEmail()))
                builder.AppendLine($"User-Email        : {context.User.GetEmail()}");

            if (!string.IsNullOrEmpty(context.User.GetDocument()))
                builder.AppendLine($"User-Document     : {context.User.GetDocument()}");
        }
        else
        {
            builder.AppendLine($"IsAuthenticated       : {false}");

            if (!string.IsNullOrEmpty(context.Request.GetUserId()))
                builder.AppendLine($"User-ID           : {context.Request.GetUserId()}");

            if (!string.IsNullOrEmpty(context.Request.GetUserAccountCode()))
                builder.AppendLine($"User-Account-Code : {context.Request.GetUserAccountCode()}");

            if (!string.IsNullOrEmpty(context.Request.GetUserAccount()))
                builder.AppendLine($"User-Account      : {context.Request.GetUserAccount()}");

            if (!string.IsNullOrEmpty(context.Request.GetUserName()))
                builder.AppendLine($"User-Name         : {context.Request.GetUserName()}");

            if (!string.IsNullOrEmpty(context.Request.GetUserId()))
                builder.AppendLine($"User-Document     : {context.Request.GetUserId()}");
        }

        builder.AppendLine("====================================================================================================");

        Console.Write(builder.ToString());
    }
}
