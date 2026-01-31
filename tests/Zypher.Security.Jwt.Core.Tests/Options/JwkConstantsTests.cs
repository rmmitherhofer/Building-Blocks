using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Security.Jwt.Core;

namespace Zypher.Security.Jwt.Core.Tests.Options;

public class JwkConstantsTests
{
    [Fact(DisplayName =
        "Given JwkConstants, " +
        "When cache keys are accessed, " +
        "Then they are stable")]
    [Trait("Type", nameof(JwkConstants))]
    public async Task JwkConstants_CacheKeys_AreSet()
    {
        //Given
        //When
        var current = JwkConstants.CurrentJwkCache;
        var jwks = JwkConstants.JwksCache;

        //Then
        current.Should().Be("ZYPHER-CURRENT-SECURITY-KEY");
        jwks.Should().Be("ZYPHER-JWKS");
        await Task.CompletedTask;
    }
}
