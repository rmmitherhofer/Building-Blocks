using System;
using FluentAssertions;
using Xunit;
using Zypher.Http.Attributes;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class UrlExtensionsTests
{
    private sealed class UrlQueryModel
    {
        public string? Name { get; set; }

        [QueryStringProperty("User Id")]
        public Guid UserId { get; set; }
    }

    private sealed class EmptyQueryModel
    {
        public string? Name { get; set; }
    }

    [Fact(DisplayName =
        "Given a null or whitespace url, " +
        "When AddQueryString is called, " +
        "Then it returns the original url")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_NullOrWhitespace_ReturnsOriginal()
    {
        //Given
        var url = "   ";
        var queryParams = new { Name = "John" };

        //When
        var result = url.AddQueryString(queryParams);

        //Then
        result.Should().Be(url, "url was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url without query, " +
        "When AddQueryString is called, " +
        "Then it appends the query string")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_NoExistingQuery_Appends()
    {
        //Given
        var url = "https://example.com/users";
        var model = new UrlQueryModel
        {
            Name = "John Doe",
            UserId = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c")
        };

        //When
        var result = url.AddQueryString(model);

        //Then
        result.Should().Be(
            "https://example.com/users?name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url with existing query, " +
        "When AddQueryString is called, " +
        "Then it appends with an ampersand")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_WithExistingQuery_Appends()
    {
        //Given
        var url = "https://example.com/users?active=true";
        var model = new UrlQueryModel
        {
            Name = "John Doe",
            UserId = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c")
        };

        //When
        var result = url.AddQueryString(model);

        //Then
        result.Should().Be(
            "https://example.com/users?active=true&name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url ending with '?', " +
        "When AddQueryString is called, " +
        "Then it appends without an extra ampersand")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_EndsWithQuestionMark_AppendsCleanly()
    {
        //Given
        var url = "https://example.com/users?";
        var model = new UrlQueryModel
        {
            Name = "John Doe",
            UserId = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c")
        };

        //When
        var result = url.AddQueryString(model);

        //Then
        result.Should().Be(
            "https://example.com/users?name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url and empty query params, " +
        "When AddQueryString is called, " +
        "Then it returns the original url")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_EmptyQueryParams_ReturnsOriginal()
    {
        //Given
        var url = "https://example.com/users";
        var queryParams = new EmptyQueryModel { Name = null };

        //When
        var result = url.AddQueryString(queryParams);

        //Then
        result.Should().Be(url, "url was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url with characters to escape, " +
        "When AddQueryString is called, " +
        "Then it URL-encodes values")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_EncodesSpecialCharacters()
    {
        //Given
        var url = "https://example.com/users";
        var queryParams = new { Name = "A&B" };

        //When
        var result = url.AddQueryString(queryParams);

        //Then
        result.Should().Be(
            "https://example.com/users?name=A%26B",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url and null query params, " +
        "When AddQueryString is called, " +
        "Then it returns the original url")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_NullQueryParams_ReturnsOriginal()
    {
        //Given
        var url = "https://example.com/users";
        object? queryParams = null;

        //When
        var result = url.AddQueryString(queryParams!);

        //Then
        result.Should().Be(url, "url was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url ending with '&', " +
        "When AddQueryString is called, " +
        "Then it appends without an extra ampersand")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_EndsWithAmpersand_AppendsCleanly()
    {
        //Given
        var url = "https://example.com/users?active=true&";
        var queryParams = new { Name = "John" };

        //When
        var result = url.AddQueryString(queryParams);

        //Then
        result.Should().Be(
            "https://example.com/users?active=true&name=John",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a url with a fragment, " +
        "When AddQueryString is called, " +
        "Then it appends the query before the fragment")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_String_PreservesFragment()
    {
        //Given
        var url = "https://example.com/users#section-1";
        var queryParams = new { Name = "John" };

        //When
        var result = url.AddQueryString(queryParams);

        //Then
        result.Should().Be(
            "https://example.com/users?name=John#section-1",
            "url was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri without query, " +
        "When AddQueryString is called, " +
        "Then it returns a new uri with query")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_NoExistingQuery_Appends()
    {
        //Given
        var uri = new Uri("https://example.com/users");
        var model = new UrlQueryModel
        {
            Name = "John Doe",
            UserId = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c")
        };

        //When
        var result = uri.AddQueryString(model);

        //Then
        result.Should().Be(
            new Uri("https://example.com/users?name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c"),
            "uri was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri with existing query, " +
        "When AddQueryString is called, " +
        "Then it appends additional query parameters")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_WithExistingQuery_Appends()
    {
        //Given
        var uri = new Uri("https://example.com/users?active=true");
        var model = new UrlQueryModel
        {
            Name = "John Doe",
            UserId = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c")
        };

        //When
        var result = uri.AddQueryString(model);

        //Then
        result.Should().Be(
            new Uri("https://example.com/users?active=true&name=John+Doe&user+Id=c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c"),
            "uri was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri with existing query and empty params, " +
        "When AddQueryString is called, " +
        "Then it returns the original uri")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_EmptyQueryParams_ReturnsOriginal()
    {
        //Given
        var uri = new Uri("https://example.com/users?active=true");
        var queryParams = new EmptyQueryModel { Name = null };

        //When
        var result = uri.AddQueryString(queryParams);

        //Then
        result.Should().BeSameAs(uri, "uri was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri with a fragment, " +
        "When AddQueryString is called, " +
        "Then it preserves the fragment")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_PreservesFragment()
    {
        //Given
        var uri = new Uri("https://example.com/users#section-1");
        var queryParams = new { Name = "John" };

        //When
        var result = uri.AddQueryString(queryParams);

        //Then
        result.Should().Be(
            new Uri("https://example.com/users?name=John#section-1"),
            "uri was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri with existing query ending with '&', " +
        "When AddQueryString is called, " +
        "Then it appends without an extra ampersand")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_QueryEndsWithAmpersand_AppendsCleanly()
    {
        //Given
        var uri = new Uri("https://example.com/users?active=true&");
        var queryParams = new { Name = "John" };

        //When
        var result = uri.AddQueryString(queryParams);

        //Then
        result.Should().Be(
            new Uri("https://example.com/users?active=true&name=John"),
            "uri was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri and null query params, " +
        "When AddQueryString is called, " +
        "Then it returns the original uri instance")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_NullQueryParams_ReturnsOriginal()
    {
        //Given
        var uri = new Uri("https://example.com/users");
        object? queryParams = null;

        //When
        var result = uri.AddQueryString(queryParams!);

        //Then
        result.Should().BeSameAs(uri, "uri was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a uri with query and null params, " +
        "When AddQueryString is called, " +
        "Then it returns the original uri instance")]
    [Trait("Type", nameof(UrlExtensions))]
    public async Task AddQueryString_Uri_WithQuery_NullQueryParams_ReturnsOriginal()
    {
        //Given
        var uri = new Uri("https://example.com/users?active=true");
        object? queryParams = null;

        //When
        var result = uri.AddQueryString(queryParams!);

        //Then
        result.Should().BeSameAs(uri, "uri was '{0}'", result);
        await Task.CompletedTask;
    }
}
