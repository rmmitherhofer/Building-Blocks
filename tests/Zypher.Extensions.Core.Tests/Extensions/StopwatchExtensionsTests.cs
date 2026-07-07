using System;
using System.Diagnostics;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class StopwatchExtensionsTests
{
    [Fact(DisplayName =
        "Given a new stopwatch, " +
        "When GetFormattedTime is called, " +
        "Then it returns zero time")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_Stopwatch_Zero()
    {
        //Given
        var stopwatch = new Stopwatch();

        //When
        var result = stopwatch.GetFormattedTime();

        //Then
        result.Should().Be("00:00:00.000");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given milliseconds as long, " +
        "When GetFormattedTime is called, " +
        "Then it formats the time")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_Long_Formats()
    {
        //Given
        long ms = 1234;

        //When
        var result = ms.GetFormattedTime();

        //Then
        result.Should().Be("00:00:01.234");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given milliseconds as double, " +
        "When GetFormattedTime is called, " +
        "Then it formats the time")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_Double_Formats()
    {
        //Given
        double ms = 2500.0;

        //When
        var result = ms.GetFormattedTime();

        //Then
        result.Should().Be("00:00:02.500");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given seconds, " +
        "When GetFormattedTime is called with unit, " +
        "Then it formats using the unit")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_WithUnit_Formats()
    {
        //Given
        double seconds = 1.5;

        //When
        var result = seconds.GetFormattedTime(TimeUnit.Seconds);

        //Then
        result.Should().Be("00:00:01.500");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given minutes, " +
        "When GetFormattedTime is called with unit minutes, " +
        "Then it formats correctly")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_WithMinutes_Formats()
    {
        //Given
        double minutes = 1.5;

        //When
        var result = minutes.GetFormattedTime(TimeUnit.Minutes);

        //Then
        result.Should().Be("00:01:30.000");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given hours, " +
        "When GetFormattedTime is called with unit hours, " +
        "Then it formats correctly")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_WithHours_Formats()
    {
        //Given
        double hours = 0.5;

        //When
        var result = hours.GetFormattedTime(TimeUnit.Hours);

        //Then
        result.Should().Be("00:30:00.000");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given minutes, " +
        "When ToTimeSpan is called, " +
        "Then it converts to TimeSpan")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToTimeSpan_Converts()
    {
        //Given
        double minutes = 2;

        //When
        var result = minutes.ToTimeSpan(TimeUnit.Minutes);

        //Then
        result.Should().Be(TimeSpan.FromMinutes(2));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given seconds, " +
        "When ToTimeSpan is called, " +
        "Then it converts to TimeSpan")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToTimeSpan_Seconds_Converts()
    {
        //Given
        double seconds = 3;

        //When
        var result = seconds.ToTimeSpan(TimeUnit.Seconds);

        //Then
        result.Should().Be(TimeSpan.FromSeconds(3));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given hours, " +
        "When ToTimeSpan is called, " +
        "Then it converts to TimeSpan")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToTimeSpan_Hours_Converts()
    {
        //Given
        double hours = 1.25;

        //When
        var result = hours.ToTimeSpan(TimeUnit.Hours);

        //Then
        result.Should().Be(TimeSpan.FromHours(1.25));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given time and unit, " +
        "When ToElapsedInfo is called, " +
        "Then it returns a filled structure")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToElapsedInfo_ReturnsInfo()
    {
        //Given
        double seconds = 1.5;

        //When
        var result = seconds.ToElapsedInfo(TimeUnit.Seconds);

        //Then
        result.ElapsedMilliseconds.Should().Be(1500);
        result.Formatted.Should().Be("00:00:01.500");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given hours, " +
        "When ToElapsedInfo is called, " +
        "Then it returns totals in hours")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToElapsedInfo_Hours_ReturnsTotals()
    {
        //Given
        double hours = 1.5;

        //When
        var result = hours.ToElapsedInfo(TimeUnit.Hours);

        //Then
        result.TotalHours.Should().Be(1.5);
        result.TotalMinutes.Should().Be(90);
        result.TotalSeconds.Should().Be(5400);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given time with decimals, " +
        "When GetRoundedFormattedTime is called, " +
        "Then it rounds and formats")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetRoundedFormattedTime_Rounds()
    {
        //Given
        double ms = 1234.56;

        //When
        var result = ms.GetRoundedFormattedTime(TimeUnit.Milliseconds, decimals: 0);

        //Then
        result.Should().Be("00:00:01.235");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given seconds, " +
        "When GetRoundedFormattedTime is called, " +
        "Then it formats based on the unit")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetRoundedFormattedTime_WithUnit_Formats()
    {
        //Given
        double seconds = 1.23;

        //When
        var result = seconds.GetRoundedFormattedTime(TimeUnit.Seconds, decimals: 0);

        //Then
        result.Should().Be("00:00:01.230");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given milliseconds and decimals, " +
        "When GetRoundedFormattedTime is called, " +
        "Then it respects rounding")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetRoundedFormattedTime_WithDecimals_RespectsRounding()
    {
        //Given
        double ms = 1234.54;

        //When
        var result = ms.GetRoundedFormattedTime(TimeUnit.Milliseconds, decimals: 1);

        //Then
        result.Should().Be("00:00:01.234");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid time unit, " +
        "When GetFormattedTime is called, " +
        "Then it throws ArgumentOutOfRangeException")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetFormattedTime_InvalidUnit_Throws()
    {
        //Given
        double ms = 1;

        //When
        Action action = () => ms.GetFormattedTime((TimeUnit)999);

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a new stopwatch, " +
        "When GetElapsedInfo is called, " +
        "Then it returns zero values")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task GetElapsedInfo_Stopwatch_Zero()
    {
        //Given
        var stopwatch = new Stopwatch();

        //When
        var result = stopwatch.GetElapsedInfo();

        //Then
        result.ElapsedMilliseconds.Should().Be(0);
        result.Formatted.Should().Be("00:00:00.000");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid time unit, " +
        "When ToTimeSpan is called, " +
        "Then it throws ArgumentOutOfRangeException")]
    [Trait("Type", nameof(StopwatchExtensions))]
    public async Task ToTimeSpan_InvalidUnit_Throws()
    {
        //Given
        double ms = 1;

        //When
        Action action = () => ms.ToTimeSpan((TimeUnit)999);

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }
}
