using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;
using Zypher.Requests.Enums;

namespace Zypher.Requests.Tests.Requests;

public class SortRequestTests
{
    [Fact(DisplayName =
        "Given a new sort request, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(SortRequest))]
    public async Task SortRequest_Defaults_AreSet()
    {
        //Given
        var request = new SortRequest();

        //When
        var direction = request.Direction;

        //Then
        direction.Should().Be(SortDirection.Asc);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a sort request, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(SortRequest))]
    public async Task SortRequest_Setters_PersistValues()
    {
        //Given
        var request = new SortRequest();

        //When
        request.Field = "createdAt";
        request.Direction = SortDirection.Desc;

        //Then
        request.Field.Should().Be("createdAt");
        request.Direction.Should().Be(SortDirection.Desc);
        await Task.CompletedTask;
    }
}
