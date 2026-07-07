using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact(DisplayName =
        "Given a valid date string, " +
        "When ToDateTimeNullable is called, " +
        "Then it returns the parsed value")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task ToDateTimeNullable_Valid_ReturnsValue()
    {
        //Given
        var input = "31/12/2023";

        //When
        var result = input.ToDateTimeNullable();

        //Then
        result.Should().Be(new DateTime(2023, 12, 31));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid date string, " +
        "When ToDateTimeNullable is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task ToDateTimeNullable_Invalid_ReturnsNull()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.ToDateTimeNullable();

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid date string, " +
        "When ToDateTime is called, " +
        "Then it returns the parsed value")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task ToDateTime_Valid_ReturnsValue()
    {
        //Given
        var input = "2024-01-02";

        //When
        var result = input.ToDateTime();

        //Then
        result.Should().Be(new DateTime(2024, 1, 2));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid date string, " +
        "When ToDateTime is called, " +
        "Then it throws FormatException")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task ToDateTime_Invalid_Throws()
    {
        //Given
        var input = "invalid";

        //When
        Action action = () => input.ToDateTime();

        //Then
        action.Should().Throw<FormatException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a supported format, " +
        "When TryParseDate is called, " +
        "Then it returns true and value")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task TryParseDate_SupportedFormat_ReturnsTrue()
    {
        //Given
        var input = "2024-01-02T03:04:05";

        //When
        var result = DateTimeExtensions.TryParseDate(input, out var parsed);

        //Then
        result.Should().BeTrue();
        parsed.Should().Be(new DateTime(2024, 1, 2, 3, 4, 5));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid date string, " +
        "When IsValidDate is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task IsValidDate_Invalid_ReturnsFalse()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.IsValidDate();

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid date string, " +
        "When IsValidDate is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(DateTimeExtensions))]
    public async Task IsValidDate_Valid_ReturnsTrue()
    {
        //Given
        var input = "2024-01-02";

        //When
        var result = input.IsValidDate();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }
}
