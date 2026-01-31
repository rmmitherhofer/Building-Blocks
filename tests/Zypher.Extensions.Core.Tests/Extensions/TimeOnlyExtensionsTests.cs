using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class TimeOnlyExtensionsTests
{
    [Fact(DisplayName =
        "Given a DateTime, " +
        "When ToTimeOnly is called, " +
        "Then it returns the time component")]
    [Trait("Type", nameof(TimeOnlyExtensions))]
    public async Task ToTimeOnly_FromDateTime_ReturnsTime()
    {
        //Given
        var dateTime = new DateTime(2024, 1, 1, 13, 45, 30);

        //When
        var result = dateTime.ToTimeOnly();

        //Then
        result.Should().Be(new TimeOnly(13, 45, 30));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a time string, " +
        "When ToTimeOnly is called, " +
        "Then it parses the time")]
    [Trait("Type", nameof(TimeOnlyExtensions))]
    public async Task ToTimeOnly_String_Parses()
    {
        //Given
        var input = "23:59:00";

        //When
        var result = input.ToTimeOnly();

        //Then
        result.Should().Be(new TimeOnly(23, 59, 0));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid time string, " +
        "When ToTimeOnly is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(TimeOnlyExtensions))]
    public async Task ToTimeOnly_String_Invalid_ReturnsNull()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.ToTimeOnly();

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a TimeOnly, " +
        "When To24H is called, " +
        "Then it formats as HH:mm")]
    [Trait("Type", nameof(TimeOnlyExtensions))]
    public async Task To24H_Formats()
    {
        //Given
        var time = new TimeOnly(9, 5, 0);

        //When
        var result = time.To24H();

        //Then
        result.Should().Be("09:05");
        await Task.CompletedTask;
    }
}
