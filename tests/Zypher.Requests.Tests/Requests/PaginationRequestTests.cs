using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;

namespace Zypher.Requests.Tests.Requests;

public class PaginationRequestTests
{
    [Fact(DisplayName =
        "Given a new pagination request, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(PaginationRequest))]
    public async Task PaginationRequest_Defaults_AreSet()
    {
        //Given
        var request = new PaginationRequest();

        //When
        var page = request.Page;

        //Then
        page.Should().Be(1);
        request.PageSize.Should().Be(10);
        request.IncludeTotalCount.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a pagination request, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(PaginationRequest))]
    public async Task PaginationRequest_Setters_PersistValues()
    {
        //Given
        var request = new PaginationRequest();

        //When
        request.Page = 2;
        request.PageSize = 25;
        request.IncludeTotalCount = false;

        //Then
        request.Page.Should().Be(2);
        request.PageSize.Should().Be(25);
        request.IncludeTotalCount.Should().BeFalse();
        await Task.CompletedTask;
    }
}
