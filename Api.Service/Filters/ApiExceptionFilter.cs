using Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Api.Service.Filters;

/// <summary>
/// Exception filter that maps known exception types to appropriate HTTP status codes.
/// </summary>
public class ApiExceptionFilter : ExceptionFilterAttribute
{
    /// <summary>
    /// Called when an exception occurs during action execution.
    /// Maps the exception to an HTTP status code based on its type.
    /// </summary>
    /// <param name="context">The exception context.</param>
    public override void OnException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = context.Exception.GetType().Name switch
        {
            nameof(DomainException) => (int)HttpStatusCode.BadRequest,
            nameof(NotFoundException) => (int)HttpStatusCode.NotFound,
            nameof(HttpRequestException) or nameof(CustomHttpRequestException) => (int)HttpStatusCode.BadGateway,
            nameof(UnauthorizedAccessException) => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError,
        };
        base.OnException(context);
    }
}
