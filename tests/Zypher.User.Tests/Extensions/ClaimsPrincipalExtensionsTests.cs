using System;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Xunit;
using Zypher.User.Extensions;

namespace Zypher.User.Tests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "test");
        return new ClaimsPrincipal(identity);
    }

    [Fact(DisplayName =
        "Given an authenticated identity, " +
        "When IsAuthenticated is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task IsAuthenticated_ReturnsTrue()
    {
        //Given
        var principal = CreatePrincipal(new Claim(ClaimsPrincipalExtensions.USER_ID, "1"));

        //When
        var result = principal.IsAuthenticated();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a claim, " +
        "When GetClaim is called, " +
        "Then it returns the claim value")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetClaim_ReturnsValue()
    {
        //Given
        var principal = CreatePrincipal(new Claim("key", "value"));

        //When
        var result = principal.GetClaim("key");

        //Then
        result.Should().Be("value");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a user and claim, " +
        "When AddClaim is called twice, " +
        "Then it does not duplicate the claim")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task AddClaim_DoesNotDuplicate()
    {
        //Given
        var principal = CreatePrincipal();

        //When
        principal.AddClaim("key", "value");
        principal.AddClaim("key", "value");

        //Then
        principal.Claims.Count(c => c.Type == "key").Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a claim, " +
        "When AddOrUpdateClaim is called, " +
        "Then it replaces existing values")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task AddOrUpdateClaim_ReplacesValues()
    {
        //Given
        var principal = CreatePrincipal(new Claim("key", "old"));

        //When
        principal.AddOrUpdateClaim("key", "new");

        //Then
        principal.Claims.Count(c => c.Type == "key").Should().Be(1);
        principal.GetClaim("key").Should().Be("new");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given claims with a key, " +
        "When RemoveClaim is called, " +
        "Then it removes all matching claims")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task RemoveClaim_RemovesAll()
    {
        //Given
        var principal = CreatePrincipal(new Claim("key", "one"), new Claim("key", "two"));

        //When
        principal.RemoveClaim("key");

        //Then
        principal.Claims.Any(c => c.Type == "key").Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given name identifier and sub claims, " +
        "When GetId is called, " +
        "Then it prefers name identifier")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetId_PrefersNameIdentifier()
    {
        //Given
        var principal = CreatePrincipal(
            new Claim(ClaimTypes.NameIdentifier, "id-1"),
            new Claim(ClaimsPrincipalExtensions.SUB, "sub-1"));

        //When
        var result = principal.GetId();

        //Then
        result.Should().Be("id-1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given name claim and identity name, " +
        "When GetName is called, " +
        "Then it prefers name claim")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetName_PrefersClaim()
    {
        //Given
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "claim") }, "test");
        identity.AddClaim(new Claim("other", "x"));
        identity.Label = "identity";
        var principal = new ClaimsPrincipal(identity);

        //When
        var result = principal.GetName();

        //Then
        result.Should().Be("claim");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given role claims, " +
        "When GetRoles is called, " +
        "Then it returns all roles")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetRoles_ReturnsRoles()
    {
        //Given
        var principal = CreatePrincipal(
            new Claim(ClaimTypes.Role, "admin"),
            new Claim(ClaimTypes.Role, "user"));

        //When
        var result = principal.GetRoles().ToList();

        //Then
        result.Should().BeEquivalentTo(new[] { "admin", "user" });
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given locale and language claims, " +
        "When GetLocale is called, " +
        "Then it prefers locale")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetLocale_PrefersLocale()
    {
        //Given
        var principal = CreatePrincipal(
            new Claim(ClaimsPrincipalExtensions.LOCALE, "pt-BR"),
            new Claim(ClaimsPrincipalExtensions.LANGUAGE, "en-US"));

        //When
        var result = principal.GetLocale();

        //Then
        result.Should().Be("pt-BR");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an expiration claim, " +
        "When GetExpiration is called, " +
        "Then it returns the UTC DateTime")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetExpiration_ParsesUnixSeconds()
    {
        //Given
        var seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var principal = CreatePrincipal(new Claim(ClaimsPrincipalExtensions.EXPIRATION, seconds.ToString()));

        //When
        var result = principal.GetExpiration();

        //Then
        result.Should().Be(DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-numeric expiration claim, " +
        "When GetExpiration is called, " +
        "Then it returns null")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetExpiration_Invalid_ReturnsNull()
    {
        //Given
        var principal = CreatePrincipal(new Claim(ClaimsPrincipalExtensions.EXPIRATION, "invalid"));

        //When
        var result = principal.GetExpiration();

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given JWT and refresh claims, " +
        "When GetToken and GetRefreshToken are called, " +
        "Then they return the values")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task TokenClaims_ReturnValues()
    {
        //Given
        var principal = CreatePrincipal(
            new Claim(ClaimsPrincipalExtensions.JWT, "access"),
            new Claim(ClaimsPrincipalExtensions.RT, "refresh"),
            new Claim(ClaimsPrincipalExtensions.RT_EXP, "exp"));

        //When
        var token = principal.GetToken();
        var refresh = principal.GetRefreshToken();
        var refreshExp = principal.GetRefreshExpires();

        //Then
        token.Should().Be("access");
        refresh.Should().Be("refresh");
        refreshExp.Should().Be("exp");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a fingerprint claim, " +
        "When GetFingerprint is called, " +
        "Then it returns the value")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task GetFingerprint_ReturnsValue()
    {
        //Given
        var principal = CreatePrincipal(new Claim(ClaimsPrincipalExtensions.FP, "fp"));

        //When
        var result = principal.GetFingerprint();

        //Then
        result.Should().Be("fp");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given claim helpers, " +
        "When Add* methods are called, " +
        "Then they add expected claims")]
    [Trait("Type", nameof(ClaimsPrincipalExtensions))]
    public async Task AddHelpers_AddClaims()
    {
        //Given
        var principal = CreatePrincipal();

        //When
        principal.AddId("id");
        principal.AddName("name");
        principal.AddEmail("email");
        principal.AddRole("role");
        principal.AddTenantId("tenant");
        principal.AddCompanyId("company");
        principal.AddDepartment("dept");
        principal.AddProfile("profile");
        principal.AddLocale("pt-BR");
        principal.AddExpiration("123");
        principal.AddSessionId("session");
        principal.AddDocument("doc");
        principal.AddAccount("account");
        principal.AddAccountCode("code");
        principal.AddToken("jwt");
        principal.AddRefreshToken("rt");
        principal.AddRefreshExpires("rt_exp");
        principal.AddFingerprint("fp");

        //Then
        principal.GetId().Should().Be("id");
        principal.GetName().Should().Be("name");
        principal.GetEmail().Should().Be("email");
        principal.GetRoles().Should().Contain("role");
        principal.GetTenantId().Should().Be("tenant");
        principal.GetCompanyId().Should().Be("company");
        principal.GetDepartment().Should().Be("dept");
        principal.GetProfile().Should().Be("profile");
        principal.GetLocale().Should().Be("pt-BR");
        principal.GetExpiration().Should().NotBeNull();
        principal.GetSessionId().Should().Be("session");
        principal.GetDocument().Should().Be("doc");
        principal.GetAccount().Should().Be("account");
        principal.GetAccountCode().Should().Be("code");
        principal.GetToken().Should().Be("jwt");
        principal.GetRefreshToken().Should().Be("rt");
        principal.GetRefreshExpires().Should().Be("rt_exp");
        principal.GetFingerprint().Should().Be("fp");
        await Task.CompletedTask;
    }
}
