using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;

namespace Zypher.Requests.Tests.Requests;

public class FilterRequestTests
{
    private sealed class SampleFilterRequest : FilterRequest
    {
    }

    [Fact(DisplayName =
        "Given a new filter request, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(FilterRequest))]
    public async Task FilterRequest_Defaults_AreSet()
    {
        //Given
        var request = new SampleFilterRequest();

        //When
        var pageNumber = request.PageNumber;

        //Then
        pageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
        request.OrderBy.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a filter request, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(FilterRequest))]
    public async Task FilterRequest_Setters_PersistValues()
    {
        //Given
        var request = new SampleFilterRequest();

        //When
        request.PageNumber = 3;
        request.PageSize = 50;
        request.OrderBy = "-createdAt";

        //Then
        request.PageNumber.Should().Be(3);
        request.PageSize.Should().Be(50);
        request.OrderBy.Should().Be("-createdAt");
        await Task.CompletedTask;
    }
}
