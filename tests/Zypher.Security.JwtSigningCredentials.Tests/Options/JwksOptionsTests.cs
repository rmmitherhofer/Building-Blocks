using System;
using FluentAssertions;
using Xunit;
using Zypher.Security.JwtSigningCredentials;

namespace Zypher.Security.JwtSigningCredentials.Tests.Options;

public class JwksOptionsTests
{
    [Fact(DisplayName =
        "Given default options, " +
        "When JwksOptions is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(JwksOptions))]
    public async Task JwksOptions_Defaults_AreSet()
    {
        //Given
        var options = new JwksOptions();

        //When
        var algorithm = options.Algorithm;

        //Then
        algorithm.Should().Be(Algorithm.ES256);
        options.DaysUntilExpire.Should().Be(90);
        options.KeyPrefix.Should().Contain(Environment.MachineName);
        options.AlgorithmsToKeep.Should().Be(2);
        await Task.CompletedTask;
    }
}
