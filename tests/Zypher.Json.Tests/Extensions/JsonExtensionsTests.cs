using System;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Zypher.Json;

namespace Zypher.Json.Tests.Extensions;

public class JsonExtensionsTests
{
    private sealed class Sample
    {
        public string? Name { get; set; }
    }

    private sealed class NonSerializable
    {
        public NonSerializable Self => this;
    }

    [Fact(DisplayName =
        "Given an object, " +
        "When Serialize is called, " +
        "Then it returns JSON string")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task Serialize_ReturnsJson()
    {
        //Given
        var data = new Sample { Name = "John" };

        //When
        var result = JsonExtensions.Serialize(data);

        //Then
        result.Should().Contain("\"name\"");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When Serialize is called, " +
        "Then it returns the JSON null literal")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task Serialize_Invalid_Throws()
    {
        //Given
        object? data = null;

        //When
        var result = JsonExtensions.Serialize(data!);

        //Then
        result.Should().Be("null");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JSON string, " +
        "When Deserialize is called, " +
        "Then it returns the object")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task Deserialize_ReturnsObject()
    {
        //Given
        var json = "{\"name\":\"John\"}";

        //When
        var result = JsonExtensions.Deserialize<Sample>(json);

        //Then
        result.Should().NotBeNull();
        result!.Name.Should().Be("John");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty JSON string, " +
        "When Deserialize is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task Deserialize_Empty_ReturnsNull()
    {
        //Given
        var json = string.Empty;

        //When
        var result = JsonExtensions.Deserialize<Sample>(json);

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given invalid JSON, " +
        "When Deserialize is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task Deserialize_Invalid_Throws()
    {
        //Given
        var json = "{";

        //When
        Action action = () => JsonExtensions.Deserialize<Sample>(json);

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given JSON, " +
        "When TryDeserialize is called, " +
        "Then it returns true and sets result")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryDeserialize_ReturnsTrue()
    {
        //Given
        var json = "{\"name\":\"John\"}";

        //When
        var success = JsonExtensions.TryDeserialize(json, out Sample? result);

        //Then
        success.Should().BeTrue();
        result.Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given invalid JSON, " +
        "When TryDeserialize is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryDeserialize_Invalid_ReturnsFalse()
    {
        //Given
        var json = "{";

        //When
        var success = JsonExtensions.TryDeserialize(json, out Sample? result);

        //Then
        success.Should().BeFalse();
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given raw JSON, " +
        "When TryFormatJson is called, " +
        "Then it returns a formatted JSON")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryFormatJson_Formats()
    {
        //Given
        var raw = "{\"name\":\"John\"}";

        //When
        var result = JsonExtensions.TryFormatJson(raw);

        //Then
        result.Should().Contain(Environment.NewLine);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given invalid JSON, " +
        "When TryFormatJson is called, " +
        "Then it returns the original string")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryFormatJson_Invalid_ReturnsOriginal()
    {
        //Given
        var raw = "{";

        //When
        var result = JsonExtensions.TryFormatJson(raw);

        //Then
        result.Should().Be(raw);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JSON string, " +
        "When TryFormatJson is called, " +
        "Then it unwraps and formats")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryFormatJson_WrappedString_Formats()
    {
        //Given
        var wrapped = "\"{\\\"name\\\":\\\"John\\\"}\"";

        //When
        var result = JsonExtensions.TryFormatJson(wrapped);

        //Then
        result.Should().Contain(Environment.NewLine);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null or whitespace JSON, " +
        "When TryFormatJson is called, " +
        "Then it returns the original input")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task TryFormatJson_Empty_ReturnsOriginal()
    {
        //Given
        var raw = " ";

        //When
        var result = JsonExtensions.TryFormatJson(raw);

        //Then
        result.Should().Be(raw);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object, " +
        "When SerializeContent is called, " +
        "Then it returns StringContent with JSON")]
    [Trait("Type", nameof(JsonExtensions))]
    public async Task SerializeContent_ReturnsStringContent()
    {
        //Given
        var data = new Sample { Name = "John" };

        //When
        var content = JsonExtensions.SerializeContent(data);
        var body = await content.ReadAsStringAsync();

        //Then
        content.Headers.ContentType?.MediaType.Should().Be(JsonExtensions.CONTENT_TYPE);
        body.Should().Contain("\"name\"");
    }
}
