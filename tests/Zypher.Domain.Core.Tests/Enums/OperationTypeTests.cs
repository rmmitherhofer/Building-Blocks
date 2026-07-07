using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.Enums;

namespace Zypher.Domain.Core.Tests.Enums;

public class OperationTypeTests
{
    [Fact(DisplayName =
        "Given OperationType enum values, " +
        "When they are cast to int, " +
        "Then they match expected values")]
    [Trait("Type", nameof(OperationType))]
    public async Task OperationType_Values_AreExpected()
    {
        //Given
        var none = (int)OperationType.None;
        var added = (int)OperationType.Added;
        var modified = (int)OperationType.Modified;
        var deleted = (int)OperationType.Deleted;

        //When
        //Then
        none.Should().Be(0);
        added.Should().Be(1);
        modified.Should().Be(2);
        deleted.Should().Be(3);
        await Task.CompletedTask;
    }
}
