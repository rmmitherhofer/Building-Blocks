using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Zypher.Api.Data.Extensions;

namespace Zypher.Api.Data.Tests.Extensions;

public class QueryExtensionsTests
{
    [Fact(DisplayName =
        "Given page number and size, " +
        "When Page is called, " +
        "Then it returns the correct slice")]
    [Trait("Type", nameof(QueryExtensions))]
    public async Task Page_ValidParameters_ReturnsSlice()
    {
        //Given
        var source = Enumerable.Range(1, 25).AsQueryable();

        //When
        var result = source.Page(2, 10).ToList();

        //Then
        result.Should().Equal(Enumerable.Range(11, 10));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given invalid page values, " +
        "When Page is called, " +
        "Then it uses defaults")]
    [Trait("Type", nameof(QueryExtensions))]
    public async Task Page_InvalidParameters_UsesDefaults()
    {
        //Given
        var source = Enumerable.Range(1, 15).AsQueryable();

        //When
        var result = source.Page(0, 0).ToList();

        //Then
        result.Should().Equal(Enumerable.Range(1, 10));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a page beyond total, " +
        "When Page is called, " +
        "Then it returns empty")]
    [Trait("Type", nameof(QueryExtensions))]
    public async Task Page_BeyondTotal_ReturnsEmpty()
    {
        //Given
        var source = Enumerable.Range(1, 5).AsQueryable();

        //When
        var result = source.Page(2, 10).ToList();

        //Then
        result.Should().BeEmpty();
        await Task.CompletedTask;
    }
}
