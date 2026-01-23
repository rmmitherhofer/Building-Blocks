using FluentAssertions;
using Xunit;
using Zypher.Security.JwtSigningCredentials.Interfaces;
using Zypher.Security.JwtSigningCredentials.Jwk;
using Zypher.Security.JwtSigningCredentials.Jwks;
using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Tests.Jwks;

public class JwksServiceTests
{
    private sealed class FakeStore : IJsonWebKeyStore
    {
        private readonly List<SecurityKeyWithPrivate> _store = new();
        private SecurityKeyWithPrivate _current;

        public void Save(SecurityKeyWithPrivate securityParameters)
        {
            _store.Add(securityParameters);
            _current = securityParameters;
        }

        public SecurityKeyWithPrivate GetCurrentKey() => _current;
        public IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5) => _store.AsReadOnly();
        public void Clear() => _store.Clear();
        public bool NeedsUpdate() => _current == null;
        public void Update(SecurityKeyWithPrivate securityKeyWithPrivate)
        {
            var index = _store.FindIndex(x => x.Id == securityKeyWithPrivate.Id);
            if (index >= 0) _store[index] = securityKeyWithPrivate;
        }
    }

    [Fact(DisplayName =
        "Given empty store, " +
        "When GetLastKeysCredentials is called, " +
        "Then it generates and returns keys")]
    [Trait("Type", nameof(JwksService))]
    public async Task GetLastKeysCredentials_EmptyStore_Generates()
    {
        //Given
        var store = new FakeStore();
        var options = Microsoft.Extensions.Options.Options.Create(new JwksOptions { AlgorithmsToKeep = 1 });
        var jwkService = new JwkService();
        var service = new JwksService(store, options, jwkService);

        //When
        var result = service.GetLastKeysCredentials(1);

        //Then
        result.Should().NotBeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given store needs update, " +
        "When GetCurrent is called, " +
        "Then it generates a key")]
    [Trait("Type", nameof(JwksService))]
    public async Task GetCurrent_NeedsUpdate_Generates()
    {
        //Given
        var store = new FakeStore();
        var options = Microsoft.Extensions.Options.Options.Create(new JwksOptions { AlgorithmsToKeep = 1 });
        var jwkService = new JwkService();
        var service = new JwksService(store, options, jwkService);

        //When
        var result = service.GetCurrent();

        //Then
        result.Should().NotBeNull();
        await Task.CompletedTask;
    }
}
