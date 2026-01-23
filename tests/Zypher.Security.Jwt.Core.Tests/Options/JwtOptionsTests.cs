using System;
using FluentAssertions;
using Xunit;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Jwa;

namespace Zypher.Security.Jwt.Core.Tests.Options;

public class JwtOptionsTests
{
    [Fact(DisplayName =
        "Given default JwtOptions, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(JwtOptions))]
    public async Task JwtOptions_Defaults_AreSet()
    {
        //Given
        var options = new JwtOptions();

        //When
        var jws = options.Jws;

        //Then
        jws.Should().NotBeNull();
        options.Jwe.Should().NotBeNull();
        options.DaysUntilExpire.Should().Be(90);
        options.KeyPrefix.Should().Contain(Environment.MachineName);
        options.AlgorithmToKeep.Should().Be(2);
        options.CacheTime.Should().Be(TimeSpan.FromMinutes(15));
        jws.JwtType.Should().Be(JwtType.Jws);
        await Task.CompletedTask;
    }
}
