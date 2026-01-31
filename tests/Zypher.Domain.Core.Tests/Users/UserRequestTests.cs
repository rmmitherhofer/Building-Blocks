using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Domain.Core.Users;

namespace Zypher.Domain.Core.Tests.Users;

public class UserRequestTests
{
    [Fact(DisplayName =
        "Given an http context, " +
        "When UserRequest is created, " +
        "Then it maps request data")]
    [Trait("Type", nameof(UserRequest))]
    public async Task UserRequest_MapsContextData()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers["X-User-Id"] = "1";
        context.Request.Headers["X-User-Name"] = "Renato";
        context.Request.Headers["X-User-Email"] = "renato@zypher.com";
        context.Request.Headers["User-Agent"] = "test-agent";
        context.Request.Headers["X-Request-ID"] = "req-1";
        context.Request.Headers["X-Correlation-ID"] = "corr-1";
        context.Request.Headers["X-Client-Id"] = "client-1";
        context.Request.Headers["X-Pod-Name"] = "pod-1";
        context.Request.Headers["Referer"] = "https://example.com";
        context.Request.Method = "GET";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/users";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "renato@zypher.com")
        }, "test"));

        //When
        var request = new UserRequest(context);

        //Then
        request.UserId.Should().Be("1");
        request.Name.Should().Be("Renato");
        request.Email.Should().Be("renato@zypher.com");
        request.IpAddress.Should().Be("127.0.0.1");
        request.UserAgent.Should().Be("test-agent");
        request.SessionRequest.RequestId.Should().Be("req-1");
        request.SessionRequest.CorrelationId.Should().Be("corr-1");
        request.SessionRequest.ClientId.Should().Be("client-1");
        request.SessionRequest.Method.Should().Be("GET");
        request.SessionRequest.Url.Should().Be("https://example.com/users");
        request.SessionRequest.PodName.Should().Be("pod-1");
        request.SessionRequest.Referer.Should().Be("https://example.com");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given missing correlation headers, " +
        "When UserRequest is created, " +
        "Then it generates request and correlation ids")]
    [Trait("Type", nameof(UserRequest))]
    public async Task UserRequest_GeneratesRequestAndCorrelationIds()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/health";

        //When
        var request = new UserRequest(context);

        //Then
        request.SessionRequest.RequestId.Should().NotBeNullOrEmpty();
        request.SessionRequest.CorrelationId.Should().NotBeNullOrEmpty();
        request.SessionRequest.RequestId.Should().Be(request.SessionRequest.CorrelationId);
        await Task.CompletedTask;
    }
}
