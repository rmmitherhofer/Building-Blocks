using System;
using System.Globalization;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class NumberExtensionsTests
{
    [Fact(DisplayName =
        "Given a null decimal, " +
        "When ToPtBr is called, " +
        "Then it returns 0.00")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task ToPtBr_Null_ReturnsZero()
    {
        //Given
        decimal? value = null;

        //When
        var result = value.ToPtBr();

        //Then
        result.Should().Be("0.00");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a decimal, " +
        "When ToPtBr is called, " +
        "Then it formats using pt-BR")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task ToPtBr_Formats()
    {
        //Given
        var value = 1234.5m;

        //When
        var result = value.ToPtBr();

        //Then
        result.Should().Be("1.234,50");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a culture, " +
        "When FormatNumber is called, " +
        "Then it formats using the culture")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task FormatNumber_UsesCulture()
    {
        //Given
        var value = 1234.5m;
        var culture = new CultureInfo("en-US");

        //When
        var result = value.FormatNumber(culture, decimalPlaces: 1);

        //Then
        result.Should().Be("1,234.5");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null culture, " +
        "When FormatNumber is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task FormatNumber_NullCulture_Throws()
    {
        //Given
        var value = 1m;

        //When
        Action action = () => value.FormatNumber(null!);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a currency string, " +
        "When ParseToDecimal is called, " +
        "Then it parses using culture")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task ParseToDecimal_Parses()
    {
        //Given
        var value = "$1,234.56";
        var culture = new CultureInfo("en-US");

        //When
        var result = value.ParseToDecimal(culture);

        //Then
        result.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or whitespace string, " +
        "When ParseToDecimal is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task ParseToDecimal_Null_ReturnsNull()
    {
        //Given
        string? value = null;

        //When
        var result = value.ParseToDecimal();

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid string, " +
        "When ParseToDecimal is called, " +
        "Then it throws FormatException")]
    [Trait("Type", nameof(NumberExtensions))]
    public async Task ParseToDecimal_Invalid_Throws()
    {
        //Given
        var value = "invalid";

        //When
        Action action = () => value.ParseToDecimal(new CultureInfo("pt-BR"));

        //Then
        action.Should().Throw<FormatException>();
        await Task.CompletedTask;
    }
}
