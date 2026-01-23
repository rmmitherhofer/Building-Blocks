using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.JwtExtensions;

namespace Zypher.Security.JwtExtensions.Tests.Models;

public class JwkListTests
{
    [Fact(DisplayName =
        "Given a JsonWebKeySet, " +
        "When JwkList is created, " +
        "Then it sets properties")]
    [Trait("Type", nameof(JwkList))]
    public async Task JwkList_Ctor_SetsProperties()
    {
        //Given
        var jwks = new JsonWebKeySet("{\"keys\":[]}");

        //When
        var list = new JwkList(jwks);

        //Then
        list.Jwks.Should().BeSameAs(jwks);
        list.When.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        await Task.CompletedTask;
    }
}
