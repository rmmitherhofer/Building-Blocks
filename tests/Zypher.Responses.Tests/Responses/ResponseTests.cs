using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class ResponseTests
{
    private sealed class SampleResponse : Response
    {
    }

    [Fact(DisplayName =
        "Given a derived response, " +
        "When it is created, " +
        "Then it is a Response instance")]
    [Trait("Type", nameof(Response))]
    public async Task Response_Derived_IsResponse()
    {
        //Given
        var response = new SampleResponse();

        //When
        var isResponse = response is Response;

        //Then
        isResponse.Should().BeTrue();
        await Task.CompletedTask;
    }
}
