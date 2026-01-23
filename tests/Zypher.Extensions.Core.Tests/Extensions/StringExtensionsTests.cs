using System;
using FluentAssertions;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact(DisplayName =
        "Given a CPF string, " +
        "When CpfMask is called, " +
        "Then it applies the CPF mask")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CpfMask_Formats()
    {
        //Given
        var cpf = "12345678901";

        //When
        var result = cpf.CpfMask();

        //Then
        result.Should().Be("123.456.789-01");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid CPF length, " +
        "When CpfMask is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CpfMask_InvalidLength_Throws()
    {
        //Given
        var cpf = "123456789012";

        //When
        Action action = () => cpf.CpfMask();

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a CNPJ string, " +
        "When CnpjMask is called, " +
        "Then it applies the CNPJ mask")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CnpjMask_Formats()
    {
        //Given
        var cnpj = "12345678000199";

        //When
        var result = cnpj.CnpjMask();

        //Then
        result.Should().Be("12.345.678/0001-99");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid CNPJ length, " +
        "When CnpjMask is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CnpjMask_InvalidLength_Throws()
    {
        //Given
        var cnpj = "12345678901";

        //When
        Action action = () => cnpj.CnpjMask();

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a CPF or CNPJ string, " +
        "When CpfCnpjMask is called, " +
        "Then it applies the correct mask")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CpfCnpjMask_AppliesCorrectMask()
    {
        //Given
        var cpf = "12345678901";
        var cnpj = "12345678000199";

        //When
        var cpfResult = cpf.CpfCnpjMask();
        var cnpjResult = cnpj.CpfCnpjMask();

        //Then
        cpfResult.Should().Be("123.456.789-01");
        cnpjResult.Should().Be("12.345.678/0001-99");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid CPF/CNPJ length, " +
        "When CpfCnpjMask is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task CpfCnpjMask_InvalidLength_Throws()
    {
        //Given
        var input = new string('1', 20);

        //When
        Action action = () => input.CpfCnpjMask();

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a masked cpf/cnpj, " +
        "When RemoveMaskCpfCnpj is called, " +
        "Then it removes mask characters")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task RemoveMaskCpfCnpj_RemovesMask()
    {
        //Given
        var input = "123.456.789-01";

        //When
        var result = input.RemoveMaskCpfCnpj();

        //Then
        result.Should().Be("12345678901");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or empty cpf/cnpj, " +
        "When RemoveMaskCpfCnpj is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task RemoveMaskCpfCnpj_NullOrEmpty_ReturnsEmpty()
    {
        //Given
        string? nullValue = null;
        var empty = string.Empty;

        //When
        var resultNull = nullValue.RemoveMaskCpfCnpj();
        var resultEmpty = empty.RemoveMaskCpfCnpj();

        //Then
        resultNull.Should().Be(string.Empty);
        resultEmpty.Should().Be(string.Empty);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given registration numbers with length 7 and 8, " +
        "When MaskRegistrationCpfCnpj is called, " +
        "Then it formats accordingly")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task MaskRegistrationCpfCnpj_Formats()
    {
        //Given
        var len7 = "1234567";
        var len8 = "12345678";

        //When
        var result7 = len7.MaskRegistrationCpfCnpj();
        var result8 = len8.MaskRegistrationCpfCnpj();

        //Then
        result7.Should().Be("001.234.567");
        result8.Should().Be("12.345.678");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid registration, " +
        "When MaskRegistrationCpfCnpj is called, " +
        "Then it throws FormatException")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task MaskRegistrationCpfCnpj_Invalid_Throws()
    {
        //Given
        var invalid = "abc";

        //When
        Action action = () => invalid.MaskRegistrationCpfCnpj();

        //Then
        action.Should().Throw<FormatException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a telephone number, " +
        "When TelephoneMask is called, " +
        "Then it formats the number")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task TelephoneMask_Formats()
    {
        //Given
        var tel11 = "11987654321";
        var tel10 = "1132654321";

        //When
        var result11 = tel11.TelephoneMask();
        var result10 = tel10.TelephoneMask();

        //Then
        result11.Should().Be("(11) 98765-4321");
        result10.Should().Be("(11) 3265-4321");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid telephone length, " +
        "When TelephoneMask is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task TelephoneMask_Invalid_ReturnsEmpty()
    {
        //Given
        var tel = "123";

        //When
        var result = tel.TelephoneMask();

        //Then
        result.Should().Be(string.Empty);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a zip code, " +
        "When ZipCodeMask is called, " +
        "Then it formats the zip code")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ZipCodeMask_Formats()
    {
        //Given
        var zip = "12345678";

        //When
        var result = zip.ZipCodeMask();

        //Then
        result.Should().Be("12345-678");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid zip code, " +
        "When ZipCodeMask is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ZipCodeMask_Invalid_ReturnsEmpty()
    {
        //Given
        var zip = "123";

        //When
        var result = zip.ZipCodeMask();

        //Then
        result.Should().Be(string.Empty);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a lower case string, " +
        "When ToTitleCase is called, " +
        "Then it converts to title case")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ToTitleCase_Converts()
    {
        //Given
        var input = "joao da silva";

        //When
        var result = input.ToTitleCase();

        //Then
        result.Should().Be("Joao Da Silva");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or empty string, " +
        "When ToTitleCase is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ToTitleCase_NullOrEmpty_ReturnsEmpty()
    {
        //Given
        string? nullValue = null;
        var empty = string.Empty;

        //When
        var resultNull = nullValue.ToTitleCase();
        var resultEmpty = empty.ToTitleCase();

        //Then
        resultNull.Should().Be(string.Empty);
        resultEmpty.Should().Be(string.Empty);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or empty string, " +
        "When EmptyIfNullOrEmpty is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task EmptyIfNullOrEmpty_ReturnsEmpty()
    {
        //Given
        string? input = null;

        //When
        var result = input.EmptyIfNullOrEmpty();

        //Then
        result.Should().Be(string.Empty);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a string with accents, " +
        "When ReplaceAccents is called, " +
        "Then it removes diacritics")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ReplaceAccents_RemovesDiacritics()
    {
        //Given
        var input = "ação";

        //When
        var result = input.ReplaceAccents();

        //Then
        result.Should().Be("acao");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a string with spaces, " +
        "When RemoveSpaces is called, " +
        "Then it removes all spaces")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task RemoveSpaces_RemovesSpaces()
    {
        //Given
        var input = "a b c";

        //When
        var result = input.RemoveSpaces();

        //Then
        result.Should().Be("abc");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a Brazilian currency string, " +
        "When TryParseBrazilianCurrency is called, " +
        "Then it parses the value")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task TryParseBrazilianCurrency_Parses()
    {
        //Given
        var input = "R$ 1.234,56";

        //When
        var result = input.TryParseBrazilianCurrency(out var value);

        //Then
        result.Should().BeTrue();
        value.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid Brazilian currency, " +
        "When TryParseBrazilianCurrency is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task TryParseBrazilianCurrency_Invalid_ReturnsFalse()
    {
        //Given
        var input = "invalid";

        //When
        var result = input.TryParseBrazilianCurrency(out var value);

        //Then
        result.Should().BeFalse();
        value.Should().Be(0m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid Brazilian currency, " +
        "When ParseBrazilianCurrency is called, " +
        "Then it returns the parsed value")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ParseBrazilianCurrency_Valid_ReturnsValue()
    {
        //Given
        var input = "R$ 1.234,56";

        //When
        var result = input.ParseBrazilianCurrency();

        //Then
        result.Should().Be(1234.56m);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid currency string, " +
        "When ParseBrazilianCurrency is called, " +
        "Then it throws FormatException")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ParseBrazilianCurrency_Invalid_Throws()
    {
        //Given
        var input = "invalid";

        //When
        Action action = () => input.ParseBrazilianCurrency();

        //Then
        action.Should().Throw<FormatException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a nullable decimal, " +
        "When ToBrazilianCurrencyString is called, " +
        "Then it formats or returns default")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ToBrazilianCurrencyString_Formats()
    {
        //Given
        decimal? value = 1m;
        decimal? nullValue = null;

        //When
        var formatted = value.ToBrazilianCurrencyString();
        var empty = nullValue.ToBrazilianCurrencyString();

        //Then
        formatted.Should().Be("R$ 1,00");
        empty.Should().Be("R$ ");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given strings, " +
        "When IsNullOrEmpty and IsNullOrWhiteSpace are called, " +
        "Then they return expected results")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task IsNullOrEmpty_IsNullOrWhiteSpace_Works()
    {
        //Given
        string? empty = string.Empty;
        string? whitespace = " ";
        string? value = "a";

        //When
        var isEmpty = empty.IsNullOrEmpty();
        var isWhite = whitespace.IsNullOrWhiteSpace();
        var isEmptyFalse = value.IsNullOrEmpty();

        //Then
        isEmpty.Should().BeTrue();
        isWhite.Should().BeTrue();
        isEmptyFalse.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a full name, " +
        "When ToInitialsAlias is called, " +
        "Then it returns initials")]
    [Trait("Type", nameof(StringExtensions))]
    public async Task ToInitialsAlias_ReturnsInitials()
    {
        //Given
        var full = "John Doe";
        var single = "John";
        var empty = "   ";

        //When
        var resultFull = full.ToInitialsAlias();
        var resultSingle = single.ToInitialsAlias();
        var resultEmpty = empty.ToInitialsAlias();

        //Then
        resultFull.Should().Be("JD");
        resultSingle.Should().Be("J");
        resultEmpty.Should().Be(string.Empty);
        await Task.CompletedTask;
    }
}
