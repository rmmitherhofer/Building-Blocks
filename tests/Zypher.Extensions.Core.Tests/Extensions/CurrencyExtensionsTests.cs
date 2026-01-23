using System;
using System.Globalization;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class CurrencyExtensionsTests
{
    [Fact(DisplayName =
        "Given a decimal and culture, " +
        "When ToCurrency is called, " +
        "Then it formats using the culture")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToCurrency_UsesCulture()
    {
        //Given
        var value = 12.34m;
        var culture = new CultureInfo("en-US");

        //When
        var result = value.ToCurrency(culture);

        //Then
        result.Should().Be("$12.34", "value was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null culture, " +
        "When ToCurrency is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToCurrency_NullCulture_Throws()
    {
        //Given
        var value = 1m;

        //When
        Action action = () => value.ToCurrency((CultureInfo)null!);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a culture code, " +
        "When ToCurrency is called, " +
        "Then it formats using the culture code")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToCurrency_WithCultureCode_Formats()
    {
        //Given
        var value = 5m;

        //When
        var result = value.ToCurrency("en-US");

        //Then
        result.Should().Be("$5.00", "value was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty culture code, " +
        "When ToCurrency is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToCurrency_EmptyCultureCode_Throws()
    {
        //Given
        var value = 5m;

        //When
        Action action = () => value.ToCurrency(string.Empty);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null decimal, " +
        "When ToCurrency is called, " +
        "Then it returns zero with currency symbol")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToCurrency_Nullable_ReturnsZero()
    {
        //Given
        decimal? value = null;
        var culture = new CultureInfo("en-US");

        //When
        var result = value.ToCurrency(culture);

        //Then
        result.Should().Be("$0.00", "value was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a currency string and culture, " +
        "When ToDecimal is called, " +
        "Then it parses the value")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToDecimal_ParsesWithCulture()
    {
        //Given
        var input = "R$ 1.234,56";
        var culture = new CultureInfo("pt-BR");

        //When
        var result = input.ToDecimal(culture);

        //Then
        result.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a currency string without culture, " +
        "When ToDecimal is called, " +
        "Then it parses using fallback cultures")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToDecimal_UsesFallbackCultures()
    {
        //Given
        var input = "$1,234.56";

        //When
        var result = input.ToDecimal();

        //Then
        result.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid currency string, " +
        "When ToDecimal is called, " +
        "Then it throws FormatException")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task ToDecimal_Invalid_Throws()
    {
        //Given
        var input = "invalid";

        //When
        Action action = () => input.ToDecimal();

        //Then
        action.Should().Throw<FormatException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid currency string, " +
        "When TryParseCurrency is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task TryParseCurrency_Invalid_ReturnsFalse()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.TryParseCurrency(out var value);

        //Then
        result.Should().BeFalse();
        value.Should().Be(0m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid currency string, " +
        "When TryParseCurrency is called, " +
        "Then it returns true and value")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task TryParseCurrency_Valid_ReturnsTrue()
    {
        //Given
        var input = "$1,234.56";

        //When
        var result = input.TryParseCurrency(out var value);

        //Then
        result.Should().BeTrue();
        value.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a currency string with symbols, " +
        "When RemoveCurrencySymbols is called, " +
        "Then it removes known symbols")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task RemoveCurrencySymbols_RemovesKnownSymbols()
    {
        //Given
        var input = "R$ 10,00";

        //When
        var result = input.RemoveCurrencySymbols();

        //Then
        result.Should().Be("10,00", "value was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null culture, " +
        "When GetCurrencySymbol is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(CurrencyExtensions))]
    public async Task GetCurrencySymbol_NullCulture_ReturnsEmpty()
    {
        //Given
        CultureInfo? culture = null;

        //When
        var result = CurrencyExtensions.GetCurrencySymbol(culture!);

        //Then
        result.Should().Be(string.Empty);
        await Task.CompletedTask;
    }
}
