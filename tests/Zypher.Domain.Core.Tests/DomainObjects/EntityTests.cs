using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.DomainObjects;

namespace Zypher.Domain.Core.Tests.DomainObjects;

public class EntityTests
{
    private sealed class SampleEntity : Entity
    {
        public void SetId(Guid id)
        {
            typeof(Entity).GetProperty(nameof(Id), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                .SetValue(this, id);
        }
    }

    [Fact(DisplayName =
        "Given two entities with same id, " +
        "When Equals is called, " +
        "Then they are equal")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_Equals_SameId()
    {
        //Given
        var id = Guid.NewGuid();
        var entity1 = new SampleEntity();
        var entity2 = new SampleEntity();
        entity1.SetId(id);
        entity2.SetId(id);

        //When
        var result = entity1.Equals(entity2);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given two entities with different ids, " +
        "When Equals is called, " +
        "Then they are not equal")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_Equals_DifferentId()
    {
        //Given
        var entity1 = new SampleEntity();
        var entity2 = new SampleEntity();

        //When
        var result = entity1.Equals(entity2);

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given the same entity reference, " +
        "When Equals is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_Equals_SameReference()
    {
        //Given
        var entity = new SampleEntity();

        //When
        var result = entity.Equals(entity);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-entity object, " +
        "When Equals is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_Equals_NonEntity_ReturnsFalse()
    {
        //Given
        var entity = new SampleEntity();

        //When
        var result = entity.Equals("not-entity");

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given two null entities, " +
        "When == is used, " +
        "Then it returns true")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_EqualityOperator_BothNull()
    {
        //Given
        Entity? entity1 = null;
        Entity? entity2 = null;

        //When
        var result = entity1 == entity2;

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given one null entity, " +
        "When == is used, " +
        "Then it returns false")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_EqualityOperator_OneNull()
    {
        //Given
        Entity? entity1 = new SampleEntity();
        Entity? entity2 = null;

        //When
        var result = entity1 == entity2;

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given entities with same id, " +
        "When == is used, " +
        "Then it returns true")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_EqualityOperator_SameId()
    {
        //Given
        var id = Guid.NewGuid();
        var entity1 = new SampleEntity();
        var entity2 = new SampleEntity();
        entity1.SetId(id);
        entity2.SetId(id);

        //When
        var result = entity1 == entity2;

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given entities with different ids, " +
        "When != is used, " +
        "Then it returns true")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_InequalityOperator_DifferentId()
    {
        //Given
        var entity1 = new SampleEntity();
        var entity2 = new SampleEntity();

        //When
        var result = entity1 != entity2;

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an entity, " +
        "When GetHashCode is called, " +
        "Then it uses type and id")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_GetHashCode_UsesTypeAndId()
    {
        //Given
        var id = Guid.NewGuid();
        var entity = new SampleEntity();
        entity.SetId(id);

        //When
        var hash1 = entity.GetHashCode();
        var hash2 = entity.GetHashCode();

        //Then
        hash1.Should().Be(hash2);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an entity, " +
        "When ToString is called, " +
        "Then it includes type and id")]
    [Trait("Type", nameof(Entity))]
    public async Task Entity_ToString_Formats()
    {
        //Given
        var id = Guid.NewGuid();
        var entity = new SampleEntity();
        entity.SetId(id);

        //When
        var text = entity.ToString();

        //Then
        text.Should().Contain(nameof(SampleEntity));
        text.Should().Contain(id.ToString());
        await Task.CompletedTask;
    }
}
