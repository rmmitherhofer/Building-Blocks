using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.Users;

namespace Zypher.Domain.Core.Tests.Users;

public class SessionRequestTests
{
    [Fact(DisplayName =
        "Given a session request, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(SessionRequest))]
    public async Task SessionRequest_Setters_PersistValues()
    {
        //Given
        var request = new SessionRequest();

        //When
        request.RequestId = "req";
        request.CorrelationId = "corr";
        request.ClientId = "client";
        request.Method = "POST";
        request.Url = "https://example.com";
        request.Referer = "https://ref";
        request.PodName = "pod";

        //Then
        request.RequestId.Should().Be("req");
        request.CorrelationId.Should().Be("corr");
        request.ClientId.Should().Be("client");
        request.Method.Should().Be("POST");
        request.Url.Should().Be("https://example.com");
        request.Referer.Should().Be("https://ref");
        request.PodName.Should().Be("pod");
        await Task.CompletedTask;
    }
}
