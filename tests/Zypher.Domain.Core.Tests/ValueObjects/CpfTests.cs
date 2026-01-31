using System;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.ValueObjects;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.Tests.ValueObjects;

public class CpfTests
{
    [Fact(DisplayName =
        "Given a valid CPF, " +
        "When Cpf is created, " +
        "Then it sets parts")]
    [Trait("Type", nameof(Cpf))]
    public async Task Cpf_Valid_SetsParts()
    {
        //Given
        var cpf = "529.982.247-25";

        //When
        var value = new Cpf(cpf);

        //Then
        value.Number.Should().Be("52998224725");
        value.Registration.Should().Be("529982247");
        value.Digit.Should().Be("25");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid CPF, " +
        "When Cpf is created, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Cpf))]
    public async Task Cpf_Invalid_Throws()
    {
        //Given
        var cpf = "123.456.789-00";

        //When
        Action action = () => _ = new Cpf(cpf);

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a CPF with non digits, " +
        "When Validar is called, " +
        "Then it ignores formatting characters")]
    [Trait("Type", nameof(Cpf))]
    public async Task Cpf_Validar_IgnoresFormatting()
    {
        //Given
        var cpf = "529.982.247-25";

        //When
        var result = Cpf.Validar(cpf);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a repetitive CPF, " +
        "When Validar is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(Cpf))]
    public async Task Cpf_Validar_Repetitive_ReturnsFalse()
    {
        //Given
        var cpf = "000.000.000-00";

        //When
        var result = Cpf.Validar(cpf);

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }
}
