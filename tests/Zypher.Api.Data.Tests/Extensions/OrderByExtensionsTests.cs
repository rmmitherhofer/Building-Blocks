using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Zypher.Api.Data.Extensions;

namespace Zypher.Api.Data.Tests.Extensions;

public class OrderByExtensionsTests
{
    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public Address Address { get; set; } = new();
    }

    private sealed class Address
    {
        public string City { get; set; } = string.Empty;
    }

    [Fact(DisplayName =
        "Given a null enumerable, " +
        "When OrderBy is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_Null_Throws()
    {
        //Given
        IEnumerable<Person>? source = null;

        //When
        Action action = () => source!.OrderBy("Name").ToList();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty order string, " +
        "When OrderBy is called, " +
        "Then it returns source as-is")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_EmptyOrder_ReturnsSource()
    {
        //Given
        var source = new List<Person> { new() { Name = "B" }, new() { Name = "A" } };

        //When
        var result = source.OrderBy(" ").ToList();

        //Then
        result.Should().BeEquivalentTo(source, options => options.WithStrictOrdering());
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a property name, " +
        "When OrderBy is called, " +
        "Then it sorts ascending")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_Ascending_Sorts()
    {
        //Given
        var source = new List<Person>
        {
            new() { Name = "B" },
            new() { Name = "A" }
        };

        //When
        var result = source.OrderBy("Name").ToList();

        //Then
        result.Select(x => x.Name).Should().ContainInOrder("A", "B");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a descending property, " +
        "When OrderBy is called, " +
        "Then it sorts descending")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_Descending_Sorts()
    {
        //Given
        var source = new List<Person>
        {
            new() { Name = "A" },
            new() { Name = "B" }
        };

        //When
        var result = source.OrderBy("-Name").ToList();

        //Then
        result.Select(x => x.Name).Should().ContainInOrder("B", "A");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given multiple properties, " +
        "When OrderBy is called, " +
        "Then it applies secondary ordering")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_MultipleProperties_Sorts()
    {
        //Given
        var source = new List<Person>
        {
            new() { Name = "B", Age = 2 },
            new() { Name = "A", Age = 2 },
            new() { Name = "A", Age = 1 }
        };

        //When
        var result = source.OrderBy("Name,-Age").ToList();

        //Then
        result.Select(x => $"{x.Name}:{x.Age}").Should().ContainInOrder("A:2", "A:1", "B:2");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a nested property, " +
        "When OrderBy is called, " +
        "Then it sorts by nested value")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_NestedProperty_Sorts()
    {
        //Given
        var source = new List<Person>
        {
            new() { Address = new Address { City = "B" } },
            new() { Address = new Address { City = "A" } }
        };

        //When
        var result = source.OrderBy("Address.City").ToList();

        //Then
        result.Select(x => x.Address.City).Should().ContainInOrder("A", "B");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid property, " +
        "When OrderBy is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Enumerable_InvalidProperty_Throws()
    {
        //Given
        var source = new List<Person> { new() { Name = "A" } };

        //When
        Action action = () => source.OrderBy("Unknown").ToList();

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null queryable, " +
        "When OrderBy is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Queryable_Null_Throws()
    {
        //Given
        IQueryable<Person>? source = null;

        //When
        Action action = () => source!.OrderBy("Name").ToList();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a queryable and empty order, " +
        "When OrderBy is called, " +
        "Then it returns source as-is")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Queryable_EmptyOrder_ReturnsSource()
    {
        //Given
        var source = new List<Person> { new() { Name = "B" }, new() { Name = "A" } }.AsQueryable();

        //When
        var result = source.OrderBy(" ").ToList();

        //Then
        result.Should().BeEquivalentTo(source.ToList(), options => options.WithStrictOrdering());
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a queryable, " +
        "When OrderBy is called, " +
        "Then it sorts by property")]
    [Trait("Type", nameof(OrderByExtensions))]
    public async Task OrderBy_Queryable_Sorts()
    {
        //Given
        var source = new List<Person>
        {
            new() { Name = "B" },
            new() { Name = "A" }
        }.AsQueryable();

        //When
        var result = source.OrderBy("Name").ToList();

        //Then
        result.Select(x => x.Name).Should().ContainInOrder("A", "B");
        await Task.CompletedTask;
    }
}
