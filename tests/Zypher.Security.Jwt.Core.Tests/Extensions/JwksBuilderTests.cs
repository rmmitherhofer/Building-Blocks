using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Security.Jwt.Core;

namespace Zypher.Security.Jwt.Core.Tests.Extensions;

public class JwksBuilderTests
{
    [Fact(DisplayName =
        "Given a null service collection, " +
        "When JwksBuilder is created, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(JwksBuilder))]
    public async Task JwksBuilder_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;

        //When
        Action action = () => _ = new JwksBuilder(services!);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a service collection, " +
        "When JwksBuilder is created, " +
        "Then it exposes the same collection")]
    [Trait("Type", nameof(JwksBuilder))]
    public async Task JwksBuilder_Services_ExposesCollection()
    {
        //Given
        var services = new ServiceCollection();

        //When
        var builder = new JwksBuilder(services);

        //Then
        builder.Services.Should().BeSameAs(services);
        await Task.CompletedTask;
    }
}
