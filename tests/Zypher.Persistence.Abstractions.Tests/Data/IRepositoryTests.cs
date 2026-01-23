using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.DomainObjects;
using Zypher.Domain.Core.Enums;
using Zypher.Persistence.Abstractions.Data;

namespace Zypher.Persistence.Abstractions.Tests.Data;

public class IRepositoryTests
{
    private sealed class SampleEntity : IAggregateRoot
    {
    }

    private sealed class SampleUnitOfWork : IUnitOfWork
    {
        public Task<(bool, OperationType)> Commit() => Task.FromResult((true, OperationType.Modified));
    }

    private sealed class SampleRepository : IRepository<SampleEntity>
    {
        public bool Added { get; private set; }
        public bool Updated { get; private set; }
        public bool Removed { get; private set; }
        public bool Disposed { get; private set; }

        public IUnitOfWork UnitOfWork { get; } = new SampleUnitOfWork();

        public Task<IEnumerable<SampleEntity>> GetAll()
            => Task.FromResult<IEnumerable<SampleEntity>>(new List<SampleEntity> { new() });

        public Task<SampleEntity?> GetById(Guid id)
            => Task.FromResult<SampleEntity?>(new SampleEntity());

        public void Add(SampleEntity entity) => Added = true;
        public void Update(SampleEntity entity) => Updated = true;
        public void Remove(SampleEntity entity) => Removed = true;

        public void Dispose() => Disposed = true;
    }

    [Fact(DisplayName =
        "Given a repository, " +
        "When methods are invoked, " +
        "Then it behaves as expected")]
    [Trait("Type", nameof(IRepository<SampleEntity>))]
    public async Task Repository_Methods_AreCallable()
    {
        //Given
        var repository = new SampleRepository();
        var entity = new SampleEntity();

        //When
        var all = await repository.GetAll();
        var byId = await repository.GetById(Guid.NewGuid());
        repository.Add(entity);
        repository.Update(entity);
        repository.Remove(entity);
        repository.Dispose();

        //Then
        all.Should().NotBeEmpty();
        byId.Should().NotBeNull();
        repository.Added.Should().BeTrue();
        repository.Updated.Should().BeTrue();
        repository.Removed.Should().BeTrue();
        repository.Disposed.Should().BeTrue();
        repository.UnitOfWork.Should().NotBeNull();
    }
}
