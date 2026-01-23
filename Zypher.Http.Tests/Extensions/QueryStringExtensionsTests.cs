using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Zypher.Http.Attributes;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class QueryStringExtensionsTests
{
    private enum Status
    {
        Active,
        Inactive
    }

    private sealed class NestedModel
    {
        public string? Value { get; set; }
    }

    private sealed class SampleModel
    {
        public string? Name { get; set; }

        public int? Age { get; set; }

        [QueryStringProperty("User Id")]
        public Guid UserId { get; set; }

        public List<int>? Items { get; set; }

        public NestedModel? Child { get; set; }
    }

    private sealed class TemplateModel
    {
        [QueryStringProperty("User Id")]
        public string? UserId { get; set; }

        public int? Age { get; set; }

        public string? Name { get; set; }

        public NestedModel? Child { get; set; }
    }

    private sealed class SimpleTypesModel
    {
        public Guid? OptionalId { get; set; }

        public Status Status { get; set; }

        public string? EmptyString { get; set; }
    }

    private sealed class TemplateSimpleTypesModel
    {
        public Guid? OptionalId { get; set; }

        public decimal? Total { get; set; }
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When ToQueryString is called, " +
        "Then it returns an empty string")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Object_Null_ReturnsEmpty()
    {
        //Given
        object? source = null;

        //When
        var result = source.ToQueryString();

        //Then
        result.Should().Be(string.Empty, "expected empty query string but got '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with attribute and complex members, " +
        "When ToQueryString is called, " +
        "Then it encodes simple non-null values and ignores complex ones")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Object_UsesAttributes_IgnoresNullAndComplex()
    {
        //Given
        var id = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c");
        var model = new SampleModel
        {
            Name = "John Doe",
            Age = null,
            UserId = id,
            Items = new List<int> { 1 },
            Child = new NestedModel { Value = "value" }
        };

        //When
        var result = model.ToQueryString();

        //Then
        result.Should().Be(
            "?name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with simple types, " +
        "When ToQueryString is called, " +
        "Then it includes nullable Guid, enum, and empty string values")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Object_IncludesSimpleTypes()
    {
        //Given
        var model = new SimpleTypesModel
        {
            OptionalId = Guid.Parse("b03c6940-1e61-4a7b-9f97-2f3a0b735c2f"),
            Status = Status.Active,
            EmptyString = string.Empty
        };

        //When
        var result = model.ToQueryString();

        //Then
        result.Should().Be(
            "?optionalId=b03c6940-1e61-4a7b-9f97-2f3a0b735c2f&status=Active&emptyString=",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a dictionary with nulls and spaces, " +
        "When ToQueryString is called, " +
        "Then it ignores nulls and URL-encodes keys and values")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Dictionary_IgnoresNull_EncodesKeysAndValues()
    {
        //Given
        IDictionary<string, object?> dict = new Dictionary<string, object?>
        {
            ["First Name"] = "Alice Smith",
            ["Age"] = 30,
            ["Empty"] = null
        };

        //When
        var result = dict.ToQueryString();

        //Then
        result.Should().Be(
            "?first+Name=Alice+Smith&age=30",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a dictionary with simple types and camelCase keys, " +
        "When ToQueryString is called, " +
        "Then it preserves camelCase and formats values")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Dictionary_SimpleTypes_And_CamelCaseKeys()
    {
        //Given
        IDictionary<string, object?> dict = new Dictionary<string, object?>
        {
            ["alreadyCamel"] = "value",
            ["status"] = Status.Inactive,
            ["id"] = Guid.Parse("9e4f414e-8f9c-4a3b-8ac8-908b2c9c1cc3")
        };

        //When
        var result = dict.ToQueryString();

        //Then
        result.Should().Be(
            "?alreadyCamel=value&status=Inactive&id=9e4f414e-8f9c-4a3b-8ac8-908b2c9c1cc3",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or empty dictionary, " +
        "When ToQueryString is called, " +
        "Then it returns an empty string")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryString_Dictionary_NullOrEmpty_ReturnsEmpty()
    {
        //Given
        IDictionary<string, object?>? dict = null;

        //When
        var nullResult = dict.ToQueryString();

        dict = new Dictionary<string, object?>();
        var emptyResult = dict.ToQueryString();

        //Then
        nullResult.Should().Be(string.Empty, "expected empty query string but got '{0}'", nullResult);
        emptyResult.Should().Be(string.Empty, "expected empty query string but got '{0}'", emptyResult);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with simple values, " +
        "When ToQueryStringTemplate is called, " +
        "Then it returns encoded placeholders for non-null simple properties")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryStringTemplate_Object_GeneratesPlaceholdersForNonNullSimple()
    {
        //Given
        var model = new TemplateModel
        {
            UserId = "abc",
            Age = 42,
            Name = null,
            Child = new NestedModel { Value = "value" }
        };

        //When
        var result = model.ToQueryStringTemplate();

        //Then
        result.Should().Be(
            "?user+Id={UserId}&age={Age}",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with all null properties, " +
        "When ToQueryStringTemplate is called, " +
        "Then it returns an empty string")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryStringTemplate_Object_AllNull_ReturnsEmpty()
    {
        //Given
        var model = new TemplateModel
        {
            UserId = null,
            Age = null,
            Name = null,
            Child = null
        };

        //When
        var result = model.ToQueryStringTemplate();

        //Then
        result.Should().Be(string.Empty, "expected empty query string but got '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with Guid and decimal values, " +
        "When ToQueryStringTemplate is called, " +
        "Then it returns placeholders for those properties")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryStringTemplate_Object_SimpleTypes_GeneratesPlaceholders()
    {
        //Given
        var model = new TemplateSimpleTypesModel
        {
            OptionalId = Guid.Parse("5a3a7f67-9ea2-4f83-8d7c-1460d0c8b7b2"),
            Total = 12.34m
        };

        //When
        var result = model.ToQueryStringTemplate();

        //Then
        result.Should().Be(
            "?optionalId={OptionalId}&total={Total}",
            "query string was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When ToQueryStringTemplate is called, " +
        "Then it returns an empty string")]
    [Trait("Type", nameof(QueryStringExtensions))]
    public async Task ToQueryStringTemplate_Object_Null_ReturnsEmpty()
    {
        //Given
        object? source = null;

        //When
        var result = source.ToQueryStringTemplate();

        //Then
        result.Should().Be(string.Empty, "expected empty query string but got '{0}'", result);
        await Task.CompletedTask;
    }
}
