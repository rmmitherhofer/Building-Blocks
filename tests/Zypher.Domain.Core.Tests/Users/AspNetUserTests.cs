using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Domain.Core.Users;
using Zypher.User.Extensions;

namespace Zypher.Domain.Core.Tests.Users;

public class AspNetUserTests
{
    private static DefaultHttpContext CreateContext(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "test");
        var principal = new ClaimsPrincipal(identity);
        return new DefaultHttpContext { User = principal };
    }

    [Fact(DisplayName =
        "Given user claims, " +
        "When AspNetUser is created, " +
        "Then it exposes claim values")]
    [Trait("Type", nameof(AspNetUser))]
    public async Task AspNetUser_Claims_MapToProperties()
    {
        //Given
        var context = CreateContext(
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Renato"),
            new Claim(ClaimTypes.Email, "renato@zypher.com"),
            new Claim(ClaimsPrincipalExtensions.USER_ACCOUNT_CODE, "AC-01"),
            new Claim(ClaimsPrincipalExtensions.USER_ACCOUNT, "Zypher"),
            new Claim(ClaimsPrincipalExtensions.USER_DOCUMENT, "123"),
            new Claim(ClaimsPrincipalExtensions.DEPARTMENT, "Engineering")
        );
        var accessor = new HttpContextAccessor { HttpContext = context };

        //When
        var user = new AspNetUser(accessor);

        //Then
        user.Id.Should().Be("1");
        user.Name.Should().Be("Renato");
        user.Email.Should().Be("renato@zypher.com");
        user.AccountCode.Should().Be("AC-01");
        user.Account.Should().Be("Zypher");
        user.Document.Should().Be("123");
        user.Departament.Should().Be("Engineering");
        user.IsAuthenticated.Should().BeTrue();
        user.GetClaims().Count().Should().Be(7);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a user role, " +
        "When IsInRole is called, " +
        "Then it returns true for matching role")]
    [Trait("Type", nameof(AspNetUser))]
    public async Task AspNetUser_IsInRole_ReturnsTrue()
    {
        //Given
        var context = CreateContext(new Claim(ClaimTypes.Role, "admin"));
        var accessor = new HttpContextAccessor { HttpContext = context };
        var user = new AspNetUser(accessor);

        //When
        var result = user.IsInRole("admin");

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unauthenticated identity, " +
        "When IsAuthenticated is checked, " +
        "Then it returns false")]
    [Trait("Type", nameof(AspNetUser))]
    public async Task AspNetUser_IsAuthenticated_False()
    {
        //Given
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        var accessor = new HttpContextAccessor { HttpContext = context };

        //When
        var user = new AspNetUser(accessor);

        //Then
        user.IsAuthenticated.Should().BeFalse();
        await Task.CompletedTask;
    }
}
