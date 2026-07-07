using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using FluentAssertions;
using Xunit;
using Zypher.Enums;
using Zypher.Enums.Resolvers;

namespace Zypher.Enums.Tests.Extensions;

public class EnumExtensionsTests
{
    private enum SampleEnum
    {
        [Description("desc-one")]
        One,

        [EnumMember(Value = "enum-member")]
        Two,

        [Display(Name = "display-name")]
        Three,

        Four
    }

    private sealed class CustomResolver : IEnumDescriptionResolver
    {
        public string? GetDescription(System.Reflection.FieldInfo field) => "custom";
    }

    private static void ResetCaches()
    {
        var descCache = typeof(EnumExtensions).GetField("_descriptionCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var lookupCache = typeof(EnumExtensions).GetField("_lookupCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        (descCache?.GetValue(null) as System.Collections.IDictionary)?.Clear();
        (lookupCache?.GetValue(null) as System.Collections.IDictionary)?.Clear();
    }

    [Fact(DisplayName =
        "Given an enum value with Description attribute, " +
        "When GetDescription is called, " +
        "Then it returns the description")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_DescriptionAttribute_ReturnsValue()
    {
        //Given
        ResetCaches();
        var value = SampleEnum.One;

        //When
        var result = value.GetDescription();

        //Then
        result.Should().Be("desc-one");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an enum value with EnumMember attribute, " +
        "When GetDescription is called, " +
        "Then it returns the enum member value")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_EnumMemberAttribute_ReturnsValue()
    {
        //Given
        ResetCaches();
        var value = SampleEnum.Two;

        //When
        var result = value.GetDescription();

        //Then
        result.Should().Be("enum-member");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an enum value with Display attribute, " +
        "When GetDescription is called, " +
        "Then it returns the display name")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_DisplayAttribute_ReturnsValue()
    {
        //Given
        ResetCaches();
        var value = SampleEnum.Three;

        //When
        var result = value.GetDescription();

        //Then
        result.Should().Be("display-name");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an enum value without attributes, " +
        "When GetDescription is called, " +
        "Then it falls back to name")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_NoAttribute_FallsBackToName()
    {
        //Given
        ResetCaches();
        var value = SampleEnum.Four;

        //When
        var result = value.GetDescription();

        //Then
        result.Should().Be("Four");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a custom resolver, " +
        "When GetDescription is called, " +
        "Then it uses the custom resolver")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_CustomResolver_UsesResolver()
    {
        //Given
        ResetCaches();
        var value = SampleEnum.Four;
        var resolver = new CustomResolver();

        //When
        var result = value.GetDescription(resolver);

        //Then
        result.Should().Be("custom");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a description, " +
        "When FromDescription is called, " +
        "Then it returns the matching enum value")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task FromDescription_ReturnsEnumValue()
    {
        //Given
        ResetCaches();
        var description = "display-name";

        //When
        var result = description.FromDescription<SampleEnum>();

        //Then
        result.Should().Be(SampleEnum.Three);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a description with different casing, " +
        "When TryFromDescription is called, " +
        "Then it matches case-insensitively")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task TryFromDescription_IsCaseInsensitive()
    {
        //Given
        ResetCaches();
        var description = "DeSc-ONe";

        //When
        var result = EnumExtensions.TryFromDescription(description, out SampleEnum value);

        //Then
        result.Should().BeTrue();
        value.Should().Be(SampleEnum.One);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unknown description, " +
        "When FromDescription is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task FromDescription_Unknown_Throws()
    {
        //Given
        ResetCaches();
        var description = "missing";

        //When
        Action action = () => description.FromDescription<SampleEnum>();

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an enum value not defined, " +
        "When GetDescription is called, " +
        "Then it returns empty string")]
    [Trait("Type", nameof(EnumExtensions))]
    public async Task GetDescription_InvalidEnumValue_ReturnsEmpty()
    {
        //Given
        ResetCaches();
        var value = (SampleEnum)999;

        //When
        var result = value.GetDescription();

        //Then
        result.Should().Be(string.Empty);
        await Task.CompletedTask;
    }
}
