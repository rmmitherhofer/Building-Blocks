using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit;
using Zypher.Security.JwtExtensions;

namespace Zypher.Security.JwtExtensions.Tests.Retrievers;

public class JwksRetrieverTests
{
    private sealed class FakeRetriever : IDocumentRetriever
    {
        private readonly string _document;
        public FakeRetriever(string document) => _document = document;
        public Task<string> GetDocumentAsync(string address, CancellationToken cancel) => Task.FromResult(_document);
    }

    [Fact(DisplayName =
        "Given a null address, " +
        "When GetConfigurationAsync is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(JwksRetriever))]
    public async Task GetConfigurationAsync_NullAddress_Throws()
    {
        //Given
        var retriever = new FakeRetriever("{\"keys\":[]}");

        //When
        Func<Task> action = async () => await JwksRetriever.GetAsync(null!, retriever, CancellationToken.None);

        //Then
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName =
        "Given a null retriever, " +
        "When GetConfigurationAsync is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(JwksRetriever))]
    public async Task GetConfigurationAsync_NullRetriever_Throws()
    {
        //Given
        IDocumentRetriever? retriever = null;

        //When
        Func<Task> action = async () => await JwksRetriever.GetAsync("https://example.com/jwks", retriever!, CancellationToken.None);

        //Then
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName =
        "Given a JWKS document, " +
        "When GetConfigurationAsync is called, " +
        "Then it returns OpenIdConnectConfiguration")]
    [Trait("Type", nameof(JwksRetriever))]
    public async Task GetConfigurationAsync_ValidDocument_ReturnsConfiguration()
    {
        //Given
        var document = "{\"keys\":[]}";
        var retriever = new FakeRetriever(document);

        //When
        OpenIdConnectConfiguration result = await JwksRetriever.GetAsync("https://example.com/jwks", retriever, CancellationToken.None);

        //Then
        result.JwksUri.Should().Be("https://example.com/jwks");
        result.JsonWebKeySet.Should().NotBeNull();
        await Task.CompletedTask;
    }
}
