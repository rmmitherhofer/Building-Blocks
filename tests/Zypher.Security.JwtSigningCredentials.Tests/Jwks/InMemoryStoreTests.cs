using FluentAssertions;
using Xunit;
using Zypher.Security.JwtSigningCredentials.Jwks;
using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Tests.Jwks;

public class InMemoryStoreTests
{
    private static InMemoryStore CreateStore()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new JwksOptions { DaysUntilExpire = 1 });
        return new InMemoryStore(options);
    }

    [Fact(DisplayName =
        "Given an empty store, " +
        "When NeedsUpdate is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(InMemoryStore))]
    public async Task NeedsUpdate_Empty_ReturnsTrue()
    {
        //Given
        var store = CreateStore();

        //When
        var result = store.NeedsUpdate();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a stored key, " +
        "When GetCurrentKey is called, " +
        "Then it returns the current key")]
    [Trait("Type", nameof(InMemoryStore))]
    public async Task GetCurrentKey_ReturnsCurrent()
    {
        //Given
        var store = CreateStore();
        var key = new SecurityKeyWithPrivate { Id = Guid.NewGuid(), CreationDate = DateTime.UtcNow };
        store.Save(key);

        //When
        var current = store.GetCurrentKey();

        //Then
        current.Should().Be(key);
        await Task.CompletedTask;
    }
}
