using Microsoft.AspNetCore.Http;
using Common.Extensions;
using Microsoft.Extensions.Logging;
using Logs.Extensions;
using System.Diagnostics;
using System.Net;
using SnapTraceV2.Factories;
using System.Runtime.ExceptionServices;
using SnapTraceV2.Decorators;
using SnapTraceV2.Helpers;
using SnapTraceV2.NotifyListeners;
using SnapTraceV2.Services;

namespace SnapTraceV2.Middlewares;

public class SnapTraceMiddleware
{
    private readonly ILogger<SnapTraceMiddleware> _logger;
    private readonly RequestDelegate _next;
    private Stopwatch _diagnostic;

    static SnapTraceMiddleware()
    {
        InternalHelper.WrapInTryCatch(ModuleInitializer.InitWhithProvider);

        LoggerService.Factory = new Factories.LoggerFactory();
    }

    public SnapTraceMiddleware(RequestDelegate next, ILogger<SnapTraceMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var httpRequest = HttpRequestFactory.Create(context.Request);

        var factory = new Factories.LoggerFactory();

        var logger = factory.GetInstance(context);

        logger.DataContainer.SetHttpProperties(new(httpRequest));

        InternalHelper.WrapInTryCatch(() => NotifyBeginRequest.Notify(httpRequest));

        ExceptionDispatchInfo? ex = null;

        if (context.Response.Body is not null && context.Response.Body is not MirrorStreamDecorator)
            context.Response.Body = new MirrorStreamDecorator(context.Response.Body);

        _diagnostic = new();
        _diagnostic.Start();

        try
        {
            SetUserClaims(context);

            _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Request: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}", context);

            await _next(context);
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            _diagnostic.Stop();

            _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Response: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)} - Timer request: {_diagnostic.ElapsedMilliseconds.GetTime()} - Status code: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}", context);

            MirrorStreamDecorator? responseStream = GetResponseStream(context.Response);

            long contentLength = responseStream is null ? 0 : responseStream.MirrorStream.Length;

            int statusCode = context.Response.StatusCode;

            if (ex is not null)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                //logger.Error(ex.SourceException);
            }

            var httpResponse = HttpResponseFactory.Create(context.Response, contentLength);

            httpResponse.SetStatusCode(statusCode);

            logger.DataContainer.HttpProperties.SetResponse(httpResponse);

            if (responseStream != null)
            {
                if (InternalHelper.CanReadResponseBody(httpResponse.Properties.Headers))
                {
                    if (ShouldLogResponseBody(logger, factory, context))
                    {
                        ILogResponseBodyStrategy logResponseBody = LogResponseBodyStrategyFactory.Create(responseStream.MirrorStream, responseStream.Encoding, logger);
                        logResponseBody.Execute();
                    }
                }

                responseStream.MirrorStream.Dispose();
            }

            IEnumerable<LoggerService> loggers = factory.GetAll(context);

            InternalHelper.WrapInTryCatch(() => NotifyFlush.Notify(loggers.ToArray()));

        }
    }

    private MirrorStreamDecorator? GetResponseStream(HttpResponse response)
    {
        if (response.Body is not null && response.Body is MirrorStreamDecorator stream)
        {
            if (!stream.MirrorStream.CanRead) return null;

            return stream;
        }

        return null;
    }

    private bool ShouldLogResponseBody(LoggerService logger, Factories.LoggerFactory loggerFactory, HttpContext httpContext)
    {
        var loggers = loggerFactory.GetAll(httpContext);

        bool? explicitValue = InternalHelper.GetExplicitLogResponseBody(loggers);

        return explicitValue ?? SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogResponseBody.Invoke(logger.DataContainer.HttpProperties);
    }

    private void SetUserClaims(HttpContext context)
    {
        context.AddClaimUserCode(context.Request.Headers[HttpContextExtensions.USER_CODE]);

        context.AddClaimUserId(context.Request.Headers[HttpContextExtensions.USER_ID]);

        context.AddClaimSessionId(context.Request.Headers[HttpContextExtensions.USER_SESSION_ID]);

        if (string.IsNullOrEmpty(context.GetSessionId()))
            context.AddClaimSessionId();

        context.AddClaimIpAddress(context.Request.Headers[HttpContextExtensions.USER_IP_ADDRESS]);

        if (string.IsNullOrEmpty(context.GetIpAddress()))
            context.AddClaimIpAddress(context.GetIpAddressForwarded());
    }
}
