using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Core.DomainObjects;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.Tests.DomainObjects;

public class ValidateTests
{
    [Fact(DisplayName =
        "Given equal objects, " +
        "When IsEquals is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsEquals_Equal_Throws()
    {
        //Given
        var value = "a";

        //When
        Action action = () => Validate.IsEquals(value, "a", "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given different objects, " +
        "When IsEquals is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsEquals_Different_DoesNotThrow()
    {
        //Given
        var value = "a";

        //When
        Action action = () => Validate.IsEquals(value, "b", "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given different objects, " +
        "When IsDifferent is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsDifferent_Different_Throws()
    {
        //Given
        var value = "a";

        //When
        Action action = () => Validate.IsDifferent(value, "b", "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given equal objects, " +
        "When IsDifferent is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsDifferent_Equal_DoesNotThrow()
    {
        //Given
        var value = "a";

        //When
        Action action = () => Validate.IsDifferent(value, "a", "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that does not match pattern, " +
        "When IsDifferent is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsDifferent_PatternMismatch_Throws()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.IsDifferent(pattern, "abc", "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that matches pattern, " +
        "When IsDifferent is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsDifferent_PatternMatch_DoesNotThrow()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.IsDifferent(pattern, "123", "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null string, " +
        "When IsEmpty is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsEmpty_Null_Throws()
    {
        //Given
        string? value = null;

        //When
        Action action = () => Validate.IsEmpty(value!, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-empty string, " +
        "When IsEmpty is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsEmpty_Value_DoesNotThrow()
    {
        //Given
        var value = "value";

        //When
        Action action = () => Validate.IsEmpty(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When IsNull is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNull_Null_Throws()
    {
        //Given
        object? value = null;

        //When
        Action action = () => Validate.IsNull(value!, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-null object, " +
        "When IsNull is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNull_Value_DoesNotThrow()
    {
        //Given
        var value = new object();

        //When
        Action action = () => Validate.IsNull(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a default struct, " +
        "When IsNullOrDefault is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrDefault_DefaultStruct_Throws()
    {
        //Given
        var value = default(Guid);

        //When
        Action action = () => Validate.IsNullOrDefault(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-default struct, " +
        "When IsNullOrDefault is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrDefault_NonDefaultStruct_DoesNotThrow()
    {
        //Given
        var value = Guid.NewGuid();

        //When
        Action action = () => Validate.IsNullOrDefault(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null collection, " +
        "When IsNullOrEmpty is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrEmpty_NullCollection_Throws()
    {
        //Given
        List<string>? items = null;

        //When
        Action action = () => Validate.IsNullOrEmpty(items!, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty collection, " +
        "When IsNullOrEmpty is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrEmpty_EmptyCollection_Throws()
    {
        //Given
        var items = new List<string>();

        //When
        Action action = () => Validate.IsNullOrEmpty(items, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-empty collection, " +
        "When IsNullOrEmpty is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrEmpty_NonEmptyCollection_DoesNotThrow()
    {
        //Given
        var items = new List<string> { "a" };

        //When
        Action action = () => Validate.IsNullOrEmpty(items, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value shorter than minimum, " +
        "When MinorOrMajor is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task MinorOrMajor_String_ThrowsWhenOutside()
    {
        //Given
        var value = "ab";

        //When
        Action action = () => Validate.MinorOrMajor(value, 3, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value inside limits, " +
        "When MinorOrMajor is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task MinorOrMajor_String_DoesNotThrow()
    {
        //Given
        var value = "abcd";

        //When
        Action action = () => Validate.MinorOrMajor(value, 3, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value between min and max, " +
        "When Between is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task Between_String_ThrowsWhenInside()
    {
        //Given
        var value = "abcd";

        //When
        Action action = () => Validate.Between(value, 3, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value outside min and max, " +
        "When Between is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task Between_String_DoesNotThrow()
    {
        //Given
        var value = "ab";

        //When
        Action action = () => Validate.Between(value, 3, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than minimum, " +
        "When LessThan is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThan_String_ThrowsWhenBelow()
    {
        //Given
        var value = "ab";

        //When
        Action action = () => Validate.LessThan(value, 3, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value equal to minimum, " +
        "When LessThan is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThan_String_DoesNotThrow()
    {
        //Given
        var value = "abc";

        //When
        Action action = () => Validate.LessThan(value, 3, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than or equal to minimum, " +
        "When LessThanOrEqual is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThanOrEqual_String_ThrowsWhenAtOrBelow()
    {
        //Given
        var value = "abc";

        //When
        Action action = () => Validate.LessThanOrEqual(value, 3, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than minimum, " +
        "When LessThanOrEqual is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThanOrEqual_String_DoesNotThrow()
    {
        //Given
        var value = "abcd";

        //When
        Action action = () => Validate.LessThanOrEqual(value, 3, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than maximum, " +
        "When GreaterThan is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThan_String_ThrowsWhenAbove()
    {
        //Given
        var value = "abcdef";

        //When
        Action action = () => Validate.GreaterThan(value, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value equal to maximum, " +
        "When GreaterThan is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThan_String_DoesNotThrow()
    {
        //Given
        var value = "abcde";

        //When
        Action action = () => Validate.GreaterThan(value, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than or equal to maximum, " +
        "When GreaterThanOrEqual is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThanOrEqual_String_ThrowsWhenAtOrAbove()
    {
        //Given
        var value = "abcde";

        //When
        Action action = () => Validate.GreaterThanOrEqual(value, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than maximum, " +
        "When GreaterThanOrEqual is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThanOrEqual_String_DoesNotThrow()
    {
        //Given
        var value = "abcd";

        //When
        Action action = () => Validate.GreaterThanOrEqual(value, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given false value, " +
        "When IsFalse is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsFalse_False_Throws()
    {
        //Given
        var value = false;

        //When
        Action action = () => Validate.IsFalse(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given true value, " +
        "When IsFalse is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsFalse_True_DoesNotThrow()
    {
        //Given
        var value = true;

        //When
        Action action = () => Validate.IsFalse(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given true value, " +
        "When IsTrue is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsTrue_True_Throws()
    {
        //Given
        var value = true;

        //When
        Action action = () => Validate.IsTrue(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given false value, " +
        "When IsTrue is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsTrue_False_DoesNotThrow()
    {
        //Given
        var value = false;

        //When
        Action action = () => Validate.IsTrue(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value not in list, " +
        "When NotContains is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task NotContains_NotInList_Throws()
    {
        //Given
        var list = new[] { "a", "b" };

        //When
        Action action = () => Validate.NotContains("c", list, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value in list, " +
        "When NotContains is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task NotContains_InList_DoesNotThrow()
    {
        //Given
        var list = new[] { "a", "b" };

        //When
        Action action = () => Validate.NotContains("a", list, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value in list, " +
        "When Contains is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task Contains_InList_Throws()
    {
        //Given
        var list = new[] { "a", "b" };

        //When
        Action action = () => Validate.Contains("a", list, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value not in list, " +
        "When Contains is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task Contains_NotInList_DoesNotThrow()
    {
        //Given
        var list = new[] { "a", "b" };

        //When
        Action action = () => Validate.Contains("c", list, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that does not match regex, " +
        "When MatchRegex is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task MatchRegex_NoMatch_Throws()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.MatchRegex(pattern, "abc", "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that matches regex, " +
        "When MatchRegex is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task MatchRegex_Match_DoesNotThrow()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.MatchRegex(pattern, "123", "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that matches regex, " +
        "When NotMatchRegex is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task NotMatchRegex_Match_Throws()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.NotMatchRegex(pattern, "123", "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value that does not match regex, " +
        "When NotMatchRegex is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task NotMatchRegex_NoMatch_DoesNotThrow()
    {
        //Given
        const string pattern = "^[0-9]+$";

        //When
        Action action = () => Validate.NotMatchRegex(pattern, "abc", "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a negative value, " +
        "When IsNegative is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNegative_Negative_Throws()
    {
        //Given
        var value = -1;

        //When
        Action action = () => Validate.IsNegative(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given zero value, " +
        "When IsNegative is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNegative_Zero_DoesNotThrow()
    {
        //Given
        var value = 0;

        //When
        Action action = () => Validate.IsNegative(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a positive value, " +
        "When IsPositive is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsPositive_Positive_Throws()
    {
        //Given
        var value = 1;

        //When
        Action action = () => Validate.IsPositive(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given zero value, " +
        "When IsPositive is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task IsPositive_Zero_DoesNotThrow()
    {
        //Given
        var value = 0;

        //When
        Action action = () => Validate.IsPositive(value, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value outside min and max, " +
        "When MinorOrMajor<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task MinorOrMajor_Generic_ThrowsWhenOutside()
    {
        //Given
        var value = 1;

        //When
        Action action = () => Validate.MinorOrMajor(value, 2, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value inside min and max, " +
        "When MinorOrMajor<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task MinorOrMajor_Generic_DoesNotThrow()
    {
        //Given
        var value = 3;

        //When
        Action action = () => Validate.MinorOrMajor(value, 2, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value between min and max, " +
        "When Between<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task Between_Generic_ThrowsWhenInside()
    {
        //Given
        var value = 3;

        //When
        Action action = () => Validate.Between(value, 2, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value outside min and max, " +
        "When Between<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task Between_Generic_DoesNotThrow()
    {
        //Given
        var value = 1;

        //When
        Action action = () => Validate.Between(value, 2, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than minimum, " +
        "When LessThan<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThan_Generic_ThrowsWhenBelow()
    {
        //Given
        var value = 1;

        //When
        Action action = () => Validate.LessThan(value, 2, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value equal to minimum, " +
        "When LessThan<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThan_Generic_DoesNotThrow()
    {
        //Given
        var value = 2;

        //When
        Action action = () => Validate.LessThan(value, 2, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than or equal to minimum, " +
        "When LessThanOrEqual<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThanOrEqual_Generic_ThrowsWhenAtOrBelow()
    {
        //Given
        var value = 2;

        //When
        Action action = () => Validate.LessThanOrEqual(value, 2, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than minimum, " +
        "When LessThanOrEqual<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task LessThanOrEqual_Generic_DoesNotThrow()
    {
        //Given
        var value = 3;

        //When
        Action action = () => Validate.LessThanOrEqual(value, 2, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than maximum, " +
        "When GreaterThan<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThan_Generic_ThrowsWhenAbove()
    {
        //Given
        var value = 6;

        //When
        Action action = () => Validate.GreaterThan(value, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value equal to maximum, " +
        "When GreaterThan<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThan_Generic_DoesNotThrow()
    {
        //Given
        var value = 5;

        //When
        Action action = () => Validate.GreaterThan(value, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value greater than or equal to maximum, " +
        "When GreaterThanOrEqual<T> is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThanOrEqual_Generic_ThrowsWhenAtOrAbove()
    {
        //Given
        var value = 5;

        //When
        Action action = () => Validate.GreaterThanOrEqual(value, 5, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a value less than maximum, " +
        "When GreaterThanOrEqual<T> is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(Validate))]
    public async Task GreaterThanOrEqual_Generic_DoesNotThrow()
    {
        //Given
        var value = 4;

        //When
        Action action = () => Validate.GreaterThanOrEqual(value, 5, "error");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null reference, " +
        "When IsNullOrDefault is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNullOrDefault_NullReference_Throws()
    {
        //Given
        string? value = null;

        //When
        Action action = () => Validate.IsNullOrDefault(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a negative decimal, " +
        "When IsNegative is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsNegative_Decimal_Throws()
    {
        //Given
        var value = -1.5m;

        //When
        Action action = () => Validate.IsNegative(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a positive decimal, " +
        "When IsPositive is called, " +
        "Then it throws DomainException")]
    [Trait("Type", nameof(Validate))]
    public async Task IsPositive_Decimal_Throws()
    {
        //Given
        var value = 1.5m;

        //When
        Action action = () => Validate.IsPositive(value, "error");

        //Then
        action.Should().Throw<DomainException>();
        await Task.CompletedTask;
    }
}
