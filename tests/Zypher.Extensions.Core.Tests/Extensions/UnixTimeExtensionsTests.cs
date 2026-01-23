using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class UnixTimeExtensionsTests
{
    [Fact(DisplayName =
        "Given a UTC date at epoch, " +
        "When ToUnixTimeSeconds is called, " +
        "Then it returns zero")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task ToUnixTimeSeconds_Epoch_ReturnsZero()
    {
        //Given
        var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //When
        var result = date.ToUnixTimeSeconds();

        //Then
        result.Should().Be(0);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given seconds, " +
        "When FromUnixTimeSeconds is called, " +
        "Then it returns the UTC date")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task FromUnixTimeSeconds_ReturnsUtcDate()
    {
        //Given
        long seconds = 1;

        //When
        var result = seconds.FromUnixTimeSeconds();

        //Then
        result.Should().Be(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a date before epoch, " +
        "When ToUnixTimeSeconds is called, " +
        "Then it throws ArgumentOutOfRangeException")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task ToUnixTimeSeconds_BeforeEpoch_Throws()
    {
        //Given
        var date = new DateTime(1969, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        //When
        Action action = () => date.ToUnixTimeSeconds();

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a negative seconds value, " +
        "When FromUnixTimeSeconds is called, " +
        "Then it throws ArgumentOutOfRangeException")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task FromUnixTimeSeconds_Negative_Throws()
    {
        //Given
        long seconds = -1;

        //When
        Action action = () => seconds.FromUnixTimeSeconds();

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a UTC date at epoch, " +
        "When ToUnixTimeMilliseconds is called, " +
        "Then it returns zero")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task ToUnixTimeMilliseconds_Epoch_ReturnsZero()
    {
        //Given
        var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //When
        var result = date.ToUnixTimeMilliseconds();

        //Then
        result.Should().Be(0);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given milliseconds, " +
        "When FromUnixTimeMilliseconds is called, " +
        "Then it returns the UTC date")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task FromUnixTimeMilliseconds_ReturnsUtcDate()
    {
        //Given
        long ms = 1000;

        //When
        var result = ms.FromUnixTimeMilliseconds();

        //Then
        result.Should().Be(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a negative milliseconds value, " +
        "When FromUnixTimeMilliseconds is called, " +
        "Then it throws ArgumentOutOfRangeException")]
    [Trait("Type", nameof(UnixTimeExtensions))]
    public async Task FromUnixTimeMilliseconds_Negative_Throws()
    {
        //Given
        long ms = -1;

        //When
        Action action = () => ms.FromUnixTimeMilliseconds();

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }
}
