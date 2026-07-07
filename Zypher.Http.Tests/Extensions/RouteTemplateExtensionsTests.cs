using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class RouteTemplateExtensionsTests
{
    private sealed class RouteModel
    {
        public int Id { get; set; }

        public string? OrderId { get; set; }
    }

    [Fact(DisplayName =
        "Given positional arguments, " +
        "When FormatRoute is called, " +
        "Then it replaces placeholders in order and escapes values")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_ReplacesInOrderAndEscapes()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";

        //When
        var result = template.FormatRoute(10, "A B");

        //Then
        result.Should().Be("/users/10/orders/A%20B", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null args array, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_NullArgs_Throws()
    {
        //Given
        var template = "/users/{id}";

        //When
        Action action = () => template.FormatRoute((object[]?)null!);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given repeated placeholders, " +
        "When FormatRoute is called, " +
        "Then it replaces all occurrences with the same value")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_RepeatedPlaceholder_ReplacesAll()
    {
        //Given
        var template = "/users/{id}/orders/{id}";

        //When
        var result = template.FormatRoute(7);

        //Then
        result.Should().Be("/users/7/orders/7", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given extra positional arguments, " +
        "When FormatRoute is called, " +
        "Then it ignores arguments without placeholders")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_ExtraArgs_IgnoresExtras()
    {
        //Given
        var template = "/users/{id}";

        //When
        var result = template.FormatRoute(7, "extra");

        //Then
        result.Should().Be("/users/7", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a simple object value, " +
        "When FormatRoute is called, " +
        "Then it replaces only the first placeholder")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Object_Simple_ReplacesFirstPlaceholder()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";

        //When
        var result = template.FormatRoute(123);

        //Then
        result.Should().Be("/users/123/orders/{orderId}", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a Guid object value, " +
        "When FormatRoute is called, " +
        "Then it replaces only the first placeholder")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Object_Guid_ReplacesFirstPlaceholder()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";
        var id = Guid.Parse("c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c");

        //When
        var result = template.FormatRoute(id);

        //Then
        result.Should().Be(
            "/users/c6f8f2a0-1f5d-4a0b-98cc-4f7b8f0d6f4c/orders/{orderId}",
            "route was '{0}'",
            result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with matching properties, " +
        "When FormatRoute is called, " +
        "Then it replaces placeholders by name")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Object_Complex_ReplacesByPropertyName()
    {
        //Given
        var template = "/users/{ID}/orders/{OrderId}";
        var model = new RouteModel { Id = 5, OrderId = "A B" };

        //When
        var result = template.FormatRoute(model);

        //Then
        result.Should().Be("/users/5/orders/A%20B", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a dictionary with missing keys, " +
        "When FormatRoute is called, " +
        "Then unmatched placeholders remain unchanged")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Dictionary_MissingKey_LeavesPlaceholder()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";
        IDictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            ["id"] = 10
        };

        //When
        var result = template.FormatRoute(parameters);

        //Then
        result.Should().Be("/users/10/orders/{orderId}", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a dictionary with null values, " +
        "When FormatRoute is called, " +
        "Then it replaces placeholders with empty strings")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Dictionary_NullValue_ReplacesWithEmpty()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";
        IDictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            ["id"] = 10,
            ["orderid"] = null
        };

        //When
        var result = template.FormatRoute(parameters);

        //Then
        result.Should().Be("/users/10/orders/", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a dictionary with lower-case keys, " +
        "When FormatRoute is called, " +
        "Then it matches placeholders case-insensitively")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Dictionary_CaseInsensitiveKeys_Replaces()
    {
        //Given
        var template = "/users/{ID}";
        IDictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            ["id"] = 10
        };

        //When
        var result = template.FormatRoute(parameters);

        //Then
        result.Should().Be("/users/10", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty template, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_EmptyTemplate_Throws()
    {
        //Given
        var template = string.Empty;

        //When
        Action action = () => template.FormatRoute(1);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null template, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Params_NullTemplate_Throws()
    {
        //Given
        string? template = null;

        //When
        Action action = () => template!.FormatRoute(1);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null template and object parameters, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Object_NullTemplate_Throws()
    {
        //Given
        string? template = null;

        //When
        Action action = () => template!.FormatRoute(new { Id = 1 });

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null template and dictionary parameters, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Dictionary_NullTemplate_Throws()
    {
        //Given
        string? template = null;
        IDictionary<string, object?> parameters = new Dictionary<string, object?> { ["id"] = 1 };

        //When
        Action action = () => template!.FormatRoute(parameters);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null dictionary, " +
        "When FormatRoute is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Dictionary_NullParameters_Throws()
    {
        //Given
        var template = "/users/{id}";

        //When
        Action action = () => template.FormatRoute((IDictionary<string, object?>)null!);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a template without placeholders and an object, " +
        "When FormatRoute is called, " +
        "Then it returns the template unchanged")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRoute_Object_NoPlaceholder_ReturnsTemplate()
    {
        //Given
        var template = "/users";

        //When
        var result = template.FormatRoute(new { Id = 1 });

        //Then
        result.Should().Be("/users", "route was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a template and arguments, " +
        "When FormatRouteWithTemplate is called, " +
        "Then it returns the original template and the formatted route")]
    [Trait("Type", nameof(RouteTemplateExtensions))]
    public async Task FormatRouteWithTemplate_Params_ReturnsTemplateAndUri()
    {
        //Given
        var template = "/users/{id}/orders/{orderId}";

        //When
        var result = template.FormatRouteWithTemplate(10, "A B");

        //Then
        result.template.Should().Be(template, "route was '{0}'", result.uri);
        result.uri.Should().Be("/users/10/orders/A%20B", "route was '{0}'", result.uri);
        await Task.CompletedTask;
    }
}
