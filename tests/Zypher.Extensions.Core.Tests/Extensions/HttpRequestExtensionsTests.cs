using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    private sealed class NonReadableStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get => 0; set { } }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    [Fact(DisplayName =
        "Given a request with forwarded header, " +
        "When GetIpAddress is called, " +
        "Then it returns the first forwarded address")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetIpAddress_UsesForwardedHeader()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.FORWARDED] = "1.2.3.4, 5.6.7.8";

        //When
        var result = context.Request.GetIpAddress();

        //Then
        result.Should().Be("1.2.3.4");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with loopback IP, " +
        "When GetIpAddress is called, " +
        "Then it returns localhost and sets forwarded header")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetIpAddress_Loopback_ReturnsLocalHost()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.IPv6Loopback;

        //When
        var result = context.Request.GetIpAddress();

        //Then
        result.Should().Be("127.0.0.1");
        context.Request.Headers[HttpRequestExtensions.FORWARDED].ToString().Should().Be("127.0.0.1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request without correlation id, " +
        "When GetCorrelationId is called, " +
        "Then it uses the request id")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetCorrelationId_UsesRequestId()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID] = "req-123";

        //When
        var result = context.Request.GetCorrelationId();

        //Then
        result.Should().Be("req-123");
        context.Request.Headers[HttpRequestExtensions.CORRELATION_ID].ToString().Should().Be("req-123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with content type parameters, " +
        "When GetContentType is called, " +
        "Then it returns only the media type")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetContentType_StripsParameters()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json; charset=utf-8";

        //When
        var result = context.Request.GetContentType();

        //Then
        result.Should().Be("application/json");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with JSON content type, " +
        "When IsJsonContent is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task IsJsonContent_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json; charset=utf-8";

        //When
        var result = context.Request.IsJsonContent();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with X-Requested-With header, " +
        "When IsAjaxRequest is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task IsAjaxRequest_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.REQUESTED_WITH] = "XMLHttpRequest";

        //When
        var result = context.Request.IsAjaxRequest();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an HTTPS request, " +
        "When IsSecureConnection is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task IsSecureConnection_Https_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";

        //When
        var result = context.Request.IsSecureConnection();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with form content type, " +
        "When IsFormContent is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task IsFormContent_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/x-www-form-urlencoded";

        //When
        var result = context.Request.IsFormContent();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request without body, " +
        "When HasBody is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task HasBody_NoContent_ReturnsFalse()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.ContentLength = 0;
        context.Request.Body = new NonReadableStream();

        //When
        var result = context.Request.HasBody();

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with query string, " +
        "When GetQueryDictionary is called, " +
        "Then it returns query values")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetQueryDictionary_ReturnsValues()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?a=1&b=two");
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
        {
            ["a"] = "1",
            ["b"] = "two"
        });

        //When
        var result = context.Request.GetQueryDictionary();

        //Then
        result.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["a"] = "1",
            ["b"] = "two"
        });
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given request data, " +
        "When GetFullUrl is called, " +
        "Then it builds the full url")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetFullUrl_BuildsUrl()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/users";
        context.Request.QueryString = new QueryString("?a=1");

        //When
        var result = context.Request.GetFullUrl();

        //Then
        result.Should().Be("https://example.com/users?a=1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty header name, " +
        "When AddHeader is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task AddHeader_EmptyKey_Throws()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        Action action = () => context.Request.AddHeader(string.Empty, "value");

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddHeader is called, " +
        "Then it does not overwrite the value")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task AddHeader_DoesNotOverwrite()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test"] = "one";

        //When
        context.Request.AddHeader("X-Test", "two");

        //Then
        context.Request.Headers["X-Test"].ToString().Should().Be("one");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddOrUpdateHeader is called, " +
        "Then it updates the header value")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task AddOrUpdateHeader_UpdatesValue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test"] = "one";

        //When
        context.Request.AddOrUpdateHeader("X-Test", "two");

        //Then
        context.Request.Headers["X-Test"].ToString().Should().Be("two");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with route data, " +
        "When GetRouteInfo is called, " +
        "Then it returns controller/action")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetRouteInfo_ReturnsControllerAction()
    {
        //Given
        var context = new DefaultHttpContext();
        var routeData = new RouteData();
        routeData.Values["controller"] = "users";
        routeData.Values["action"] = "get";
        context.Features.Set<IRoutingFeature>(new RoutingFeature { RouteData = routeData });

        //When
        var result = context.Request.GetRouteInfo();

        //Then
        result.Should().Be("users/get");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request without request id, " +
        "When GetRequestId is called, " +
        "Then it creates and returns one")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetRequestId_CreatesWhenMissing()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        var result = context.Request.GetRequestId();

        //Then
        result.Should().NotBeNullOrEmpty();
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID].ToString().Should().Be(result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing request id, " +
        "When CreateRequestId is called, " +
        "Then it keeps the existing value")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task CreateRequestId_Existing_DoesNotOverwrite()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID] = "req-123";

        //When
        context.Request.CreateRequestId();

        //Then
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID].ToString().Should().Be("req-123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request body, " +
        "When ReadBodyAsStringAsync is called, " +
        "Then it reads and resets the stream position")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task ReadBodyAsStringAsync_ReadsAndResets()
    {
        //Given
        var context = new DefaultHttpContext();
        var bytes = Encoding.UTF8.GetBytes("body");
        context.Request.Body = new MemoryStream(bytes);

        //When
        var result = await context.Request.ReadBodyAsStringAsync();

        //Then
        result.Should().Be("body");
        context.Request.Body.Position.Should().Be(0);
    }

    [Fact(DisplayName =
        "Given webhook headers, " +
        "When IsWebhookRequest is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task IsWebhookRequest_ReturnsTrue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Hub-Signature"] = "sig";

        //When
        var result = context.Request.IsWebhookRequest();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request, " +
        "When GetHeader is called, " +
        "Then it returns the header value")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetHeader_ReturnsValue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test"] = "value";

        //When
        var result = context.Request.GetHeader("X-Test");

        //Then
        result.Should().Be("value");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty header key, " +
        "When GetHeader is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(HttpRequestExtensions))]
    public async Task GetHeader_EmptyKey_Throws()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        Action action = () => context.Request.GetHeader(string.Empty);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }
}
