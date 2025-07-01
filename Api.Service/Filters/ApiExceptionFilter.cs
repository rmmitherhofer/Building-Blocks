using Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Api.Service.Filters;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
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
