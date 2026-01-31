using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Persistence.Abstractions.Data.Filters;

namespace Zypher.Persistence.Abstractions.Tests.Data.Filters;

public class FilterTests
{
    private sealed class SampleFilter : Filter
    {
    }

    [Fact(DisplayName =
        "Given a new filter, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(Filter))]
    public async Task Filter_Defaults_AreSet()
    {
        //Given
        var filter = new SampleFilter();

        //When
        var pageNumber = filter.PageNumber;

        //Then
        pageNumber.Should().Be(1);
        filter.PageSize.Should().Be(10);
        filter.OrderBy.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given total records and page size, " +
        "When GetPageCount is called, " +
        "Then it returns ceiling division")]
    [Trait("Type", nameof(Filter))]
    public async Task Filter_GetPageCount_ReturnsCeiling()
    {
        //Given
        var filter = new SampleFilter { PageSize = 10 };

        //When
        var result = filter.GetPageCount(21);

        //Then
        result.Should().Be(3);
        await Task.CompletedTask;
    }
}
