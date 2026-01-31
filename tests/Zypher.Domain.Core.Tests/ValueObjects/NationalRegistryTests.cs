using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.ValueObjects;

namespace Zypher.Domain.Core.Tests.ValueObjects;

public class NationalRegistryTests
{
    private sealed class SampleRegistry : NationalRegistry
    {
        public static string CallOnlyNumbers(string input)
        {
            var method = typeof(NationalRegistry).GetMethod("OnlyNumbers", BindingFlags.NonPublic | BindingFlags.Static)!;
            return (string)method.Invoke(null, [input])!;
        }

        public static bool CallIsValid(string number)
        {
            var method = typeof(NationalRegistry).GetMethod("IsValid", BindingFlags.NonPublic | BindingFlags.Static)!;
            return (bool)method.Invoke(null, [number])!;
        }
    }

    [Fact(DisplayName =
        "Given a mixed string, " +
        "When OnlyNumbers is used, " +
        "Then it returns only digits")]
    [Trait("Type", nameof(NationalRegistry))]
    public async Task NationalRegistry_OnlyNumbers_ExtractsDigits()
    {
        //Given
        var input = "A1.B2-C3";

        //When
        var result = SampleRegistry.CallOnlyNumbers(input);

        //Then
        result.Should().Be("123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a repetitive number, " +
        "When IsValid is called, " +
        "Then it returns true for invalid sequences")]
    [Trait("Type", nameof(NationalRegistry))]
    public async Task NationalRegistry_IsValid_Repetitive_ReturnsTrue()
    {
        //Given
        var input = "00000000000";

        //When
        var result = SampleRegistry.CallIsValid(input);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-repetitive number, " +
        "When IsValid is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(NationalRegistry))]
    public async Task NationalRegistry_IsValid_NonRepetitive_ReturnsFalse()
    {
        //Given
        var input = "12345678901";

        //When
        var result = SampleRegistry.CallIsValid(input);

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }
}
