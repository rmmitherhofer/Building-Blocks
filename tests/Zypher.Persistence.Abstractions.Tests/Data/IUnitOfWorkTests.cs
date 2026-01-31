using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.Enums;
using Zypher.Persistence.Abstractions.Data;

namespace Zypher.Persistence.Abstractions.Tests.Data;

public class IUnitOfWorkTests
{
    private sealed class SampleUnitOfWork : IUnitOfWork
    {
        public Task<(bool, OperationType)> Commit() => Task.FromResult((true, OperationType.Added));
    }

    [Fact(DisplayName =
        "Given a unit of work, " +
        "When Commit is called, " +
        "Then it returns a tuple result")]
    [Trait("Type", nameof(IUnitOfWork))]
    public async Task Commit_ReturnsTuple()
    {
        //Given
        IUnitOfWork unitOfWork = new SampleUnitOfWork();

        //When
        var result = await unitOfWork.Commit();

        //Then
        result.Item1.Should().BeTrue();
        result.Item2.Should().Be(OperationType.Added);
    }
}
