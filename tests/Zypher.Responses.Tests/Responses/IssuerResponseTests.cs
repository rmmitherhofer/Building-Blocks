using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class IssuerResponseTests
{
    [Fact(DisplayName =
        "Given a response type, " +
        "When IssuerResponse is created, " +
        "Then it sets the description type")]
    [Trait("Type", nameof(IssuerResponse))]
    public async Task IssuerResponse_Ctor_SetsDescription()
    {
        //Given
        var response = new IssuerResponse(IssuerResponseType.NotFound);

        //When
        var description = response.DescriptionType;

        //Then
        response.Type.Should().Be(IssuerResponseType.NotFound);
        description.Should().Be("NotFound");
        await Task.CompletedTask;
    }
}
