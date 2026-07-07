using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class TypeExtensionsTests
{
    private enum SampleEnum
    {
        One
    }

    private sealed class ComplexType
    {
        public string? Name { get; set; }
    }

    [Fact(DisplayName =
        "Given primitive and known types, " +
        "When IsSimpleType is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(TypeExtensions))]
    public async Task IsSimpleType_Primitives_ReturnsTrue()
    {
        //Given
        var types = new[]
        {
            typeof(int),
            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(decimal),
            typeof(SampleEnum),
            typeof(int?)
        };

        //When / Then
        foreach (var type in types)
            type.IsSimpleType().Should().BeTrue();

        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a complex type, " +
        "When IsSimpleType is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(TypeExtensions))]
    public async Task IsSimpleType_Complex_ReturnsFalse()
    {
        //Given
        var type = typeof(ComplexType);

        //When
        var result = type.IsSimpleType();

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }
}
