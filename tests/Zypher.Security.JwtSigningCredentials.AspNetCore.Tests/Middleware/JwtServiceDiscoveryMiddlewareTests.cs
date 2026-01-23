using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;
using Zypher.Security.JwtSigningCredentials.AspNetCore;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore.Tests.Middleware;

public class JwtServiceDiscoveryMiddlewareTests
{
    private sealed class FakeJwtService : IJwtService
    {
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
        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null) => Task.FromResult(_keys);
        public Task RevokeKey(string keyId, string? reason = null) => throw new NotImplementedException();
        public Task<SecurityKey> GenerateNewKey() => throw new NotImplementedException();
    }

    [Fact(DisplayName =
        "Given a jwt service, " +
        "When JwtServiceDiscoveryMiddleware is invoked, " +
        "Then it writes jwks json")]
    [Trait("Type", nameof(JwtServiceDiscoveryMiddleware))]
    public async Task InvokeAsync_WritesJwks()
    {
        //Given
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new JwtServiceDiscoveryMiddleware(next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var options = Options.Create(new JwtOptions { AlgorithmToKeep = 1 });
        var jwtService = new FakeJwtService();

        //When
        await middleware.InvokeAsync(context, jwtService, options);

        //Then
        context.Response.ContentType.Should().Be("application/json");
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        body.Should().Contain("\"keys\"");
    }
}
