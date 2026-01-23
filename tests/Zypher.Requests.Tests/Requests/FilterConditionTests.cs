using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;
using Zypher.Requests.Enums;

namespace Zypher.Requests.Tests.Requests;

public class FilterConditionTests
{
    [Fact(DisplayName =
        "Given a new filter condition, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(FilterCondition))]
    public async Task FilterCondition_Setters_PersistValues()
    {
        //Given
        var condition = new FilterCondition();

        //When
        condition.Field = "name";
        condition.Operator = FilterOperator.Contains;
        condition.Value = "john";

        //Then
        condition.Field.Should().Be("name");
        condition.Operator.Should().Be(FilterOperator.Contains);
        condition.Value.Should().Be("john");
        await Task.CompletedTask;
    }
}
