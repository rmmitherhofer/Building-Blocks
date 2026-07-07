using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class PaginatedResponseTests
{
    private sealed class SamplePaginatedResponse : PaginatedResponse
    {
        public SamplePaginatedResponse(int totalRecords, int pageNumber, int pageCount, int pageSize)
            : base(totalRecords, pageNumber, pageCount, pageSize)
        {
        }
    }

    [Fact(DisplayName =
        "Given a middle page, " +
        "When PaginatedResponse is created, " +
        "Then it sets back and next pages")]
    [Trait("Type", nameof(PaginatedResponse))]
    public async Task PaginatedResponse_MiddlePage_SetsNavigation()
    {
        //Given
        var response = new SamplePaginatedResponse(totalRecords: 100, pageNumber: 2, pageCount: 5, pageSize: 10);

        //When
        var back = response.BackPage;

        //Then
        back.Should().Be(1);
        response.NextPage.Should().Be(3);
        response.PageNumber.Should().Be(2);
        response.PageCount.Should().Be(5);
        response.PageSize.Should().Be(10);
        response.TotalRecords.Should().Be(100);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given the first page, " +
        "When PaginatedResponse is created, " +
        "Then it has no back page")]
    [Trait("Type", nameof(PaginatedResponse))]
    public async Task PaginatedResponse_FirstPage_NoBackPage()
    {
        //Given
        var response = new SamplePaginatedResponse(totalRecords: 50, pageNumber: 1, pageCount: 3, pageSize: 10);

        //When
        var back = response.BackPage;

        //Then
        back.Should().BeNull();
        response.NextPage.Should().Be(2);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given the last page, " +
        "When PaginatedResponse is created, " +
        "Then it has no next page")]
    [Trait("Type", nameof(PaginatedResponse))]
    public async Task PaginatedResponse_LastPage_NoNextPage()
    {
        //Given
        var response = new SamplePaginatedResponse(totalRecords: 50, pageNumber: 3, pageCount: 3, pageSize: 10);

        //When
        var next = response.NextPage;

        //Then
        next.Should().BeNull();
        response.BackPage.Should().Be(2);
        await Task.CompletedTask;
    }
}
