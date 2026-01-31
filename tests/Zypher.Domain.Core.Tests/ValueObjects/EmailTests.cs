using System;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.ValueObjects;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.Tests.ValueObjects;

public class EmailTests
{
    [Fact(DisplayName =
        "Given a valid email, " +
        "When Email is created, " +
        "Then it sets the address")]
    [Trait("Type", nameof(Email))]
    public async Task Email_Valid_SetsAddress()
    {
        //Given
        var email = "user@example.com";

        //When
        var value = new Email(email);

        //Then
        value.Address.Should().Be(email);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null email, " +
        "When Email is created, " +
        "Then it keeps address null")]
    [Trait("Type", nameof(Email))]
    public async Task Email_Null_DoesNotThrow()
    {
        //Given
        string? email = null;

        //When
        var value = new Email(email);

        //Then
        value.Address.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid email, " +
        "When Email is created, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Email))]
    public async Task Email_Invalid_Throws()
    {
        //Given
        var email = "invalid";

        //When
        Action action = () => _ = new Email(email);

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given multiple valid emails, " +
        "When IsValids is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Email))]
    public async Task Email_IsValids_Valid_DoesNotThrow()
    {
        //Given
        var emails = "a@a.com;b@b.com";

        //When
        Action action = () => Email.IsValids(emails);

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given invalid emails, " +
        "When IsValids is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Email))]
    public async Task Email_IsValids_Invalid_Throws()
    {
        //Given
        var emails = "a@a.com;invalid";

        //When
        Action action = () => Email.IsValids(emails);

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null list, " +
        "When IsValids is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Email))]
    public async Task Email_IsValids_Null_DoesNotThrow()
    {
        //Given
        string? emails = null;

        //When
        Action action = () => Email.IsValids(emails);

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty list, " +
        "When IsValids is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Email))]
    public async Task Email_IsValids_Empty_DoesNotThrow()
    {
        //Given
        var emails = string.Empty;

        //When
        Action action = () => Email.IsValids(emails);

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }
}
