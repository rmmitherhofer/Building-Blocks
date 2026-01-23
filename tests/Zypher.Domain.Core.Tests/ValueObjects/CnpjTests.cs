using System;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.ValueObjects;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.Tests.ValueObjects;

public class CnpjTests
{
    private static string CreateAlphanumericCnpj(string baseCnpj)
    {
        const string weights = "6543298765432";
        static int CalcDigit(string input, string weights)
        {
            var sum = 0;
            var offset = weights.Length - input.Length;
            for (var i = 0; i < input.Length; i++)
            {
                var number = input[i] - '0';
                var weight = weights[offset + i] - '0';
                sum += number * weight;
            }
            return sum % 11 < 2 ? 0 : 11 - (sum % 11);
        }

        var first = CalcDigit(baseCnpj, weights);
        var second = CalcDigit(baseCnpj + first, weights);
        return baseCnpj + first + second;
    }

    [Fact(DisplayName =
        "Given a valid CNPJ, " +
        "When Cnpj is created, " +
        "Then it sets parts")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_Valid_SetsParts()
    {
        //Given
        var cnpj = "04.252.011/0001-10";

        //When
        var value = new Cnpj(cnpj);

        //Then
        value.Number.Should().Be("04252011000110");
        value.Registration.Should().Be("04252011");
        value.Branch.Should().Be("0001");
        value.Digit.Should().Be("10");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid CNPJ, " +
        "When Cnpj is created, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_Invalid_Throws()
    {
        //Given
        var cnpj = "11.111.111/1111-11";

        //When
        Action action = () => _ = new Cnpj(cnpj);

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a formatted CNPJ, " +
        "When RemoveFormatting is called, " +
        "Then it returns digits only")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_RemoveFormatting_RemovesCharacters()
    {
        //Given
        var cnpj = "04.252.011/0001-10";

        //When
        var result = Cnpj.RemoveFormatting(cnpj);

        //Then
        result.Should().Be("04252011000110");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a valid alphanumeric CNPJ, " +
        "When Cnpj is created, " +
        "Then it sets parts")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_Alphanumeric_SetsParts()
    {
        //Given
        var baseCnpj = "AB12CD34EF56";
        var cnpj = CreateAlphanumericCnpj(baseCnpj);

        //When
        var value = new Cnpj(cnpj);

        //Then
        value.Number.Should().Be(cnpj);
        value.Registration.Should().Be(cnpj[..8]);
        value.Branch.Should().Be(cnpj.Substring(8, 4));
        value.Digit.Should().Be(cnpj.Substring(12, 2));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or whitespace CNPJ, " +
        "When Validate is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_Validate_NullOrWhitespace_ReturnsFalse()
    {
        //Given
        string? cnpj = null;

        //When
        var resultNull = Cnpj.Validate(cnpj!);
        var resultWhite = Cnpj.Validate("   ");

        //Then
        resultNull.Should().BeFalse();
        resultWhite.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid length CNPJ, " +
        "When Validate is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(Cnpj))]
    public async Task Cnpj_Validate_InvalidLength_ReturnsFalse()
    {
        //Given
        var cnpj = "123";

        //When
        var result = Cnpj.Validate(cnpj);

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }
}
