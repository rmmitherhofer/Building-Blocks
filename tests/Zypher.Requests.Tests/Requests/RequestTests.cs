using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;

namespace Zypher.Requests.Tests.Requests;

public class RequestTests
{
    private sealed class SampleRequest : Request
    {
    }

    [Fact(DisplayName =
        "Given a derived request, " +
        "When it is created, " +
        "Then it is a Request instance")]
    [Trait("Type", nameof(Request))]
    public async Task Request_Derived_IsRequest()
    {
        //Given
        var request = new SampleRequest();

        //When
        var isRequest = request is Request;

        //Then
        isRequest.Should().BeTrue();
        await Task.CompletedTask;
    }
}
