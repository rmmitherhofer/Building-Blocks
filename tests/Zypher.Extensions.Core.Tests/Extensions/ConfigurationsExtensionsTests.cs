using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class ConfigurationsExtensionsTests
{
    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider WebRootFileProvider { get; set; } = default!;
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = Environment.CurrentDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = default!;
    }

    [Fact(DisplayName =
        "Given a ConfigurationManager and environment, " +
        "When Set is called, " +
        "Then it returns the same configuration instance")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Set_ReturnsSameConfigurationManager()
    {
        //Given
        var configuration = new ConfigurationManager();
        var environment = new TestEnvironment();

        //When
        var result = configuration.Set(environment);

        //Then
        result.Should().BeSameAs(configuration);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non ConfigurationManager instance, " +
        "When Set is called, " +
        "Then it throws InvalidCastException")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Set_WithWrongType_Throws()
    {
        //Given
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var environment = new TestEnvironment();

        //When
        Action action = () => configuration.Set(environment);

        //Then
        action.Should().Throw<InvalidCastException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a config with AppSettings section, " +
        "When Get is called, " +
        "Then it resolves dot keys to colon path")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Get_DefaultSection_ResolvesPath()
    {
        //Given
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AppSettings:My:Value"] = "123"
        });

        //When
        var result = configuration.Get("My.Value");

        //Then
        result.Should().Be("123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a config and empty section, " +
        "When Get is called, " +
        "Then it uses the key without section prefix")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Get_EmptySection_UsesDirectKey()
    {
        //Given
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["My:Value"] = "abc"
        });

        //When
        var result = configuration.Get("My.Value", section: string.Empty);

        //Then
        result.Should().Be("abc");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a missing key and nullable true, " +
        "When Get is called, " +
        "Then it returns null or empty")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Get_MissingKey_Nullable_ReturnsNull()
    {
        //Given
        var configuration = new ConfigurationManager();

        //When
        var result = configuration.Get("Missing.Key", nullable: true);

        //Then
        result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a missing key and nullable false, " +
        "When Get is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(ConfigurationsExtensions))]
    public async Task Get_MissingKey_NotNullable_Throws()
    {
        //Given
        var configuration = new ConfigurationManager();

        //When
        Action action = () => configuration.Get("Missing.Key");

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
