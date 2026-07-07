using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;
using Zypher.Requests.Enums;

namespace Zypher.Requests.Tests.Requests;

public class FilterGroupTests
{
    [Fact(DisplayName =
        "Given a new filter group, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(FilterGroup))]
    public async Task FilterGroup_Defaults_AreSet()
    {
        //Given
        var group = new FilterGroup();

        //When
        var logicalOperator = group.LogicalOperator;

        //Then
        logicalOperator.Should().Be(FilterLogicalOperator.And);
        group.Conditions.Should().NotBeNull().And.BeEmpty();
        group.Groups.Should().NotBeNull().And.BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a filter group, " +
        "When conditions and groups are added, " +
        "Then they are stored")]
    [Trait("Type", nameof(FilterGroup))]
    public async Task FilterGroup_AddsConditionsAndGroups()
    {
        //Given
        var group = new FilterGroup();
        var condition = new FilterCondition
        {
            Field = "age",
            Operator = FilterOperator.GreaterThan,
            Value = 18
        };
        var nested = new FilterGroup { LogicalOperator = FilterLogicalOperator.Or };

        //When
        group.Conditions.Add(condition);
        group.Groups.Add(nested);

        //Then
        group.Conditions.Should().ContainSingle().Which.Should().Be(condition);
        group.Groups.Should().ContainSingle().Which.Should().Be(nested);
        await Task.CompletedTask;
    }
}
