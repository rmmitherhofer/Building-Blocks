using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class DateOnlyExtensionsTests
{
    [Fact(DisplayName =
        "Given a DateTime, " +
        "When ToDateOnly is called, " +
        "Then it returns the date component")]
    [Trait("Type", nameof(DateOnlyExtensions))]
    public async Task ToDateOnly_FromDateTime_ReturnsDate()
    {
        //Given
        var dateTime = new DateTime(2024, 1, 15, 10, 30, 0);

        //When
        var result = dateTime.ToDateOnly();

        //Then
        result.Should().Be(new DateOnly(2024, 1, 15));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid string date, " +
        "When ToDateOnly is called, " +
        "Then it parses the date")]
    [Trait("Type", nameof(DateOnlyExtensions))]
    public async Task ToDateOnly_String_Parses()
    {
        //Given
        var input = "31/12/2023";

        //When
        var result = input.ToDateOnly();

        //Then
        result.Should().Be(new DateOnly(2023, 12, 31));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid string date, " +
        "When ToDateOnly is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(DateOnlyExtensions))]
    public async Task ToDateOnly_String_Invalid_ReturnsNull()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.ToDateOnly();

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a DateOnly, " +
        "When ToPtBr is called, " +
        "Then it formats as dd/MM/yyyy")]
    [Trait("Type", nameof(DateOnlyExtensions))]
    public async Task ToPtBr_Formats()
    {
        //Given
        var date = new DateOnly(2024, 2, 5);

        //When
        var result = date.ToPtBr();

        //Then
        result.Should().Be("05/02/2024");
        await Task.CompletedTask;
    }
}
