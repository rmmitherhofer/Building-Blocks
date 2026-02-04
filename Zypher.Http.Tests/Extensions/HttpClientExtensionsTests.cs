using FluentAssertions;
using System.Net.Http.Headers;
using System.Reflection;
using Xunit;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class HttpClientExtensionsTests
{
    [Fact(DisplayName =
        "Given a bearer token, " +
        "When AddBearerToken is called, " +
        "Then it sets the Authorization header")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddBearerToken_SetsAuthorizationHeader()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddBearerToken("token-123");

        //Then
        client.DefaultRequestHeaders.Authorization.Should().Be(
            new AuthenticationHeaderValue("Bearer", "token-123"),
            "authorization was '{0}'",
            client.DefaultRequestHeaders.Authorization);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddHeader is called, " +
        "Then it does not overwrite the value")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddHeader_DoesNotOverwriteExistingValue()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddHeader("X-Test", "value-1");
        client.AddHeader("X-Test", "value-2");

        //Then
        client.DefaultRequestHeaders.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddOrUpdateHeader is called, " +
        "Then it replaces the value")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddOrUpdateHeader_ReplacesExistingValue()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddOrUpdateHeader("X-Test", "value-1");
        client.AddOrUpdateHeader("X-Test", "value-2");

        //Then
        client.DefaultRequestHeaders.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-2");
        await Task.CompletedTask;
    }

    private static void ResetAccessor()
    {
        var field = typeof(HttpClientExtensions).GetField("_accessor", BindingFlags.Static | BindingFlags.NonPublic);
        field?.SetValue(null, null);
    }    
}
