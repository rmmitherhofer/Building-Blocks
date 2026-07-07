using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using FluentAssertions;
using Xunit;
using Zypher.Enums.Resolvers;

namespace Zypher.Enums.Tests.Resolvers;

public class ResolverTests
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

    [Fact(DisplayName =
        "Given a DescriptionAttribute, " +
        "When DescriptionAttributeResolver is used, " +
        "Then it returns the description")]
    [Trait("Type", nameof(DescriptionAttributeResolver))]
    public async Task DescriptionAttributeResolver_ReturnsDescription()
    {
        //Given
        var field = typeof(SampleEnum).GetField(nameof(SampleEnum.One))!;
        var resolver = new DescriptionAttributeResolver();

        //When
        var result = resolver.GetDescription(field);

        //Then
        result.Should().Be("desc-one");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an EnumMemberAttribute, " +
        "When EnumMemberAttributeResolver is used, " +
        "Then it returns the enum member value")]
    [Trait("Type", nameof(EnumMemberAttributeResolver))]
    public async Task EnumMemberAttributeResolver_ReturnsValue()
    {
        //Given
        var field = typeof(SampleEnum).GetField(nameof(SampleEnum.Two))!;
        var resolver = new EnumMemberAttributeResolver();

        //When
        var result = resolver.GetDescription(field);

        //Then
        result.Should().Be("enum-member");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a DisplayAttribute, " +
        "When DisplayAttributeResolver is used, " +
        "Then it returns the display name")]
    [Trait("Type", nameof(DisplayAttributeResolver))]
    public async Task DisplayAttributeResolver_ReturnsName()
    {
        //Given
        var field = typeof(SampleEnum).GetField(nameof(SampleEnum.Three))!;
        var resolver = new DisplayAttributeResolver();

        //When
        var result = resolver.GetDescription(field);

        //Then
        result.Should().Be("display-name");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a field, " +
        "When NameFallbackResolver is used, " +
        "Then it returns the field name")]
    [Trait("Type", nameof(NameFallbackResolver))]
    public async Task NameFallbackResolver_ReturnsName()
    {
        //Given
        var field = typeof(SampleEnum).GetField(nameof(SampleEnum.Four))!;
        var resolver = new NameFallbackResolver();

        //When
        var result = resolver.GetDescription(field);

        //Then
        result.Should().Be("Four");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given multiple resolvers, " +
        "When CompositeEnumDescriptionResolver is used, " +
        "Then it returns the first non-empty description")]
    [Trait("Type", nameof(CompositeEnumDescriptionResolver))]
    public async Task CompositeResolver_ReturnsFirstNonEmpty()
    {
        //Given
        var field = typeof(SampleEnum).GetField(nameof(SampleEnum.One))!;
        var resolver = new CompositeEnumDescriptionResolver(
            new NameFallbackResolver(),
            new DescriptionAttributeResolver());

        //When
        var result = resolver.GetDescription(field);

        //Then
        result.Should().Be("One");
        await Task.CompletedTask;
    }
}
