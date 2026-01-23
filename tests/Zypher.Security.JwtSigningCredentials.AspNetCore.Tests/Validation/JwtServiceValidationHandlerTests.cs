using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;
using Zypher.Security.JwtSigningCredentials.AspNetCore;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore.Tests.Validation;

public class JwtServiceValidationHandlerTests
{
    private sealed class FakeJwtService : IJwtService
    {
        public bool Called { get; private set; }
        private readonly ReadOnlyCollection<KeyMaterial> _keys;

        public FakeJwtService()
        {
            var crypto = new CryptographicKey(Algorithm.Create(AlgorithmType.RSA, JwtType.Jws));
            _keys = new ReadOnlyCollection<KeyMaterial>(new[] { new KeyMaterial(crypto) });
        }

        public Task<SecurityKey> GenerateKey() => throw new NotImplementedException();
        public Task<SecurityKey> GetCurrentSecurityKey() => throw new NotImplementedException();
        public Task<SigningCredentials> GetCurrentSigningCredentials() => throw new NotImplementedException();
        public Task<EncryptingCredentials> GetCurrentEncryptingCredentials() => throw new NotImplementedException();
        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null)
        {
            Called = true;
            return Task.FromResult(_keys);
        }
        public Task RevokeKey(string keyId, string? reason = null) => throw new NotImplementedException();
        public Task<SecurityKey> GenerateNewKey() => throw new NotImplementedException();
    }

    [Fact(DisplayName =
        "Given a token, " +
        "When ValidateToken is called, " +
        "Then it loads signing keys from service")]
    [Trait("Type", nameof(JwtServiceValidationHandler))]
    public async Task ValidateToken_LoadsKeys()
    {
        //Given
        var services = new ServiceCollection();
        var jwtService = new FakeJwtService();
        services.AddSingleton<IJwtService>(jwtService);
        var provider = services.BuildServiceProvider();
        var handler = new JwtServiceValidationHandler(provider);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false
        };

        //When
        Action action = () => handler.ValidateToken("invalid", parameters, out _);

        //Then
        action.Should().Throw<Exception>();
        jwtService.Called.Should().BeTrue();
        parameters.IssuerSigningKeys.Should().NotBeNull();
        await Task.CompletedTask;
    }
}
