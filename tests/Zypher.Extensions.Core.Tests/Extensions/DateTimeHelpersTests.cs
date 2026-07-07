using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class DateTimeHelpersTests
{
    [Fact(DisplayName =
        "Given a weekend date, " +
        "When IsWeekend is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task IsWeekend_ReturnsTrueForWeekend()
    {
        //Given
        var date = new DateTime(2024, 2, 3); // Saturday

        //When
        var result = date.IsWeekend();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a weekday date, " +
        "When IsWeekday is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task IsWeekday_ReturnsTrueForWeekday()
    {
        //Given
        var date = new DateTime(2024, 2, 5); // Monday

        //When
        var result = date.IsWeekday();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a future date, " +
        "When IsFuture is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task IsFuture_ReturnsTrue()
    {
        //Given
        var date = DateTime.Now.AddDays(1);

        //When
        var result = date.IsFuture();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a past date, " +
        "When IsPast is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task IsPast_ReturnsTrue()
    {
        //Given
        var date = DateTime.Now.AddDays(-1);

        //When
        var result = date.IsPast();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a date, " +
        "When ToPtBrDate is called, " +
        "Then it formats as dd/MM/yyyy")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task ToPtBrDate_Formats()
    {
        //Given
        var date = new DateTime(2024, 2, 5);

        //When
        var result = date.ToPtBrDate();

        //Then
        result.Should().Be("05/02/2024");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a date, " +
        "When ToPtBrDateTime is called, " +
        "Then it formats as dd/MM/yyyy HH:mm:ss")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task ToPtBrDateTime_Formats()
    {
        //Given
        var date = new DateTime(2024, 2, 5, 7, 8, 9);

        //When
        var result = date.ToPtBrDateTime();

        //Then
        result.Should().Be("05/02/2024 07:08:09");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a date, " +
        "When FirstDayOfMonth is called, " +
        "Then it returns the first day")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task FirstDayOfMonth_ReturnsFirstDay()
    {
        //Given
        var date = new DateTime(2024, 2, 15);

        //When
        var result = date.FirstDayOfMonth();

        //Then
        result.Should().Be(new DateTime(2024, 2, 1));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a date, " +
        "When LastDayOfMonth is called, " +
        "Then it returns the last day of the month")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task LastDayOfMonth_ReturnsLastDay()
    {
        //Given
        var date = new DateTime(2024, 2, 15);

        //When
        var result = date.LastDayOfMonth();

        //Then
        result.Should().Be(new DateTime(2024, 2, 29));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a birth date and reference date, " +
        "When AgeFrom is called, " +
        "Then it calculates full years correctly")]
    [Trait("Type", nameof(DateTimeHelpers))]
    public async Task AgeFrom_CalculatesAge()
    {
        //Given
        var birthDate = new DateTime(2000, 5, 10);
        var from = new DateTime(2024, 5, 9);

        //When
        var result = birthDate.AgeFrom(from);

        //Then
        result.Should().Be(23);
        await Task.CompletedTask;
    }
}
