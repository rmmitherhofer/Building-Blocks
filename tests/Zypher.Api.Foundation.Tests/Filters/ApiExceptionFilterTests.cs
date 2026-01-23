using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;
using Zypher.Api.Foundation.Filters;
using Zypher.Domain.Exceptions;
using Zypher.Http.Exceptions;

namespace Zypher.Api.Foundation.Tests.Filters;

public class ApiExceptionFilterTests
{
    private static ExceptionContext CreateContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    [Fact(DisplayName =
        "Given a DomainException, " +
        "When OnException is called, " +
        "Then it sets status 400")]
    [Trait("Type", nameof(ApiExceptionFilter))]
    public async Task OnException_DomainException_SetsBadRequest()
    {
        //Given
        var filter = new ApiExceptionFilter();
        var context = CreateContext(new DomainException("error"));

        //When
        filter.OnException(context);

        //Then
        context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a NotFoundException, " +
        "When OnException is called, " +
        "Then it sets status 404")]
    [Trait("Type", nameof(ApiExceptionFilter))]
    public async Task OnException_NotFoundException_SetsNotFound()
    {
        //Given
        var filter = new ApiExceptionFilter();
        var context = CreateContext(new NotFoundException("missing"));

        //When
        filter.OnException(context);

        //Then
        context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a CustomHttpRequestException, " +
        "When OnException is called, " +
        "Then it sets status 502")]
    [Trait("Type", nameof(ApiExceptionFilter))]
    public async Task OnException_CustomHttpRequestException_SetsBadGateway()
    {
        //Given
        var filter = new ApiExceptionFilter();
        var context = CreateContext(new CustomHttpRequestException("bad gateway"));

        //When
        filter.OnException(context);

        //Then
        context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an UnauthorizedAccessException, " +
        "When OnException is called, " +
        "Then it sets status 401")]
    [Trait("Type", nameof(ApiExceptionFilter))]
    public async Task OnException_Unauthorized_SetsUnauthorized()
    {
        //Given
        var filter = new ApiExceptionFilter();
        var context = CreateContext(new UnauthorizedAccessException("unauthorized"));

        //When
        filter.OnException(context);

        //Then
        context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unknown exception, " +
        "When OnException is called, " +
        "Then it sets status 500")]
    [Trait("Type", nameof(ApiExceptionFilter))]
    public async Task OnException_Unknown_SetsInternalServerError()
    {
        //Given
        var filter = new ApiExceptionFilter();
        var context = CreateContext(new Exception("boom"));

        //When
        filter.OnException(context);

        //Then
        context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        await Task.CompletedTask;
    }
}
