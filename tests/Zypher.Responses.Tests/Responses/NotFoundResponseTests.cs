using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class NotFoundResponseTests
{
    [Fact(DisplayName =
        "Given no detail, " +
        "When NotFoundResponse is created, " +
        "Then it sets default values")]
    [Trait("Type", nameof(NotFoundResponse))]
    public async Task NotFoundResponse_Defaults_AreSet()
    {
        //Given
        var response = new NotFoundResponse(null);

        //Then
        response.Status.Should().Be(HttpStatusCode.NotFound);
        response.Title.Should().Be("Not found");
        response.Detail.Should().Be("Query returned no results.");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a detail message, " +
        "When NotFoundResponse is created, " +
        "Then it uses the provided detail")]
    [Trait("Type", nameof(NotFoundResponse))]
    public async Task NotFoundResponse_WithDetail_UsesProvidedDetail()
    {
        //Given
        const string detail = "missing record";

        //When
        var response = new NotFoundResponse(detail);

        //Then
        response.Detail.Should().Be(detail);
        response.Status.Should().Be(HttpStatusCode.NotFound);
        response.Title.Should().Be("Not found");
        await Task.CompletedTask;
    }
}
