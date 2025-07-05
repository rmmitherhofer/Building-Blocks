using Common.Exceptions;
using System.Collections;
using System.Text.RegularExpressions;

namespace Common.Core.DomainObjects;

/// <summary>
/// Static class containing domain validation methods.
/// Throws <see cref="DomainException"/> when validations fail.
/// </summary>
public static class Validate
{
    #region Equals

    /// <summary>
    /// Checks if two objects are equal and throws exception if they are.
    /// </summary>
    public static void IsEquals(object object1, object object2, string message)
    {
        if (object1.Equals(object2))
            throw new DomainException(message);
    }

    #endregion

    #region Different

    /// <summary>
    /// Checks if two objects are different and throws exception if they are.
    /// </summary>
    public static void IsDifferent(object object1, object object2, string message)
    {
        if (!object1.Equals(object2))
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the value does NOT match the regex pattern and throws exception if it doesn't.
    /// </summary>
    public static void IsDifferent(string pattern, string value, string message)
    {
        var regex = new Regex(pattern);
        if (!regex.IsMatch(value))
            throw new DomainException(message);
    }

    #endregion

    #region Null or Empty

    /// <summary>
    /// Checks if a string is null or consists only of whitespace and throws exception.
    /// </summary>
    public static void IsEmpty(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if an object is null and throws exception.
    /// </summary>
    public static void IsNull(object object1, string message)
    {
        if (object1 == null)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if an object is null or default (for structs) and throws exception.
    /// </summary>
    public static void IsNullOrDefault<T>(T value, string message)
    {
        if (value == null || EqualityComparer<T>.Default.Equals(value, default))
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a collection is null or empty and throws exception.
    /// </summary>
    public static void IsNullOrEmpty(IEnumerable collection, string message)
    {
        if (collection == null || !collection.Cast<object>().Any())
            throw new DomainException(message);
    }

    #endregion

    #region MinMax

    /// <summary>
    /// Checks if the length of a string is outside the minimum and maximum limits and throws exception.
    /// </summary>
    public static void MinorOrMajor(string value, int min, int max, string message)
    {
        var length = value.Trim().Length;
        if (length < min || length > max)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is outside the minimum and maximum limits and throws exception.
    /// </summary>
    public static void MinorOrMajor<T>(T value, T min, T max, string message) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new DomainException(message);
    }

    #endregion

    #region Between

    /// <summary>
    /// Checks if the length of a string is between min and max (inclusive) and throws exception if it is.
    /// Useful to validate forbidden ranges.
    /// </summary>
    public static void Between(string value, int min, int max, string message)
    {
        var length = value.Trim().Length;
        if (length >= min && length <= max)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is between min and max (inclusive) and throws exception if it is.
    /// </summary>
    public static void Between<T>(T value, T min, T max, string message) where T : IComparable<T>
    {
        if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            throw new DomainException(message);
    }

    #endregion

    #region LessThan / LessThanOrEqual

    /// <summary>
    /// Checks if the length of a string is less than the minimum and throws exception.
    /// </summary>
    public static void LessThan(string value, int min, string message)
    {
        var length = value.Trim().Length;
        if (length < min)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is less than the minimum and throws exception.
    /// </summary>
    public static void LessThan<T>(T value, T min, string message) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the length of a string is less than or equal to the minimum and throws exception.
    /// </summary>
    public static void LessThanOrEqual(string value, int min, string message)
    {
        var length = value.Trim().Length;
        if (length <= min)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is less than or equal to the minimum and throws exception.
    /// </summary>
    public static void LessThanOrEqual<T>(T value, T min, string message) where T : IComparable<T>
    {
        if (value.CompareTo(min) <= 0)
            throw new DomainException(message);
    }

    #endregion

    #region GreaterThan / GreaterThanOrEqual

    /// <summary>
    /// Checks if the length of a string is greater than the maximum and throws exception.
    /// </summary>
    public static void GreaterThan(string value, int max, string message)
    {
        var length = value.Trim().Length;
        if (length > max)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is greater than the maximum and throws exception.
    /// </summary>
    public static void GreaterThan<T>(T value, T max, string message) where T : IComparable<T>
    {
        if (value.CompareTo(max) > 0)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the length of a string is greater than or equal to the maximum and throws exception.
    /// </summary>
    public static void GreaterThanOrEqual(string value, int max, string message)
    {
        var length = value.Trim().Length;
        if (length >= max)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a comparable value is greater than or equal to the maximum and throws exception.
    /// </summary>
    public static void GreaterThanOrEqual<T>(T value, T max, string message) where T : IComparable<T>
    {
        if (value.CompareTo(max) >= 0)
            throw new DomainException(message);
    }

    #endregion

    #region Boolean

    /// <summary>
    /// Checks if the value is false and throws exception.
    /// </summary>
    public static void IsFalse(bool value, string message)
    {
        if (!value)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the value is true and throws exception.
    /// </summary>
    public static void IsTrue(bool value, string message)
    {
        if (value)
            throw new DomainException(message);
    }

    #endregion

    #region Contains / NotContains

    /// <summary>
    /// Checks if the trimmed string is NOT contained in the list and throws exception.
    /// </summary>
    public static void NotContains(string value, string[] list, string message)
    {
        if (!list.Contains(value.Trim()))
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the trimmed string is contained in the list and throws exception.
    /// </summary>
    public static void Contains(string value, string[] list, string message)
    {
        if (list.Contains(value.Trim()))
            throw new DomainException(message);
    }

    #endregion

    #region Regex / Format

    /// <summary>
    /// Checks if the value does NOT match the given regex pattern and throws exception.
    /// </summary>
    public static void MatchRegex(string pattern, string value, string message)
    {
        var regex = new Regex(pattern);
        if (!regex.IsMatch(value))
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if the value matches the given regex pattern and throws exception.
    /// </summary>
    public static void NotMatchRegex(string pattern, string value, string message)
    {
        var regex = new Regex(pattern);
        if (regex.IsMatch(value))
            throw new DomainException(message);
    }

    #endregion

    #region Negative / Positive

    /// <summary>
    /// Checks if a numeric value is negative and throws exception.
    /// </summary>
    public static void IsNegative<T>(T value, string message) where T : IComparable<T>
    {
        var zero = (T)Convert.ChangeType(0, typeof(T));
        if (value.CompareTo(zero) < 0)
            throw new DomainException(message);
    }

    /// <summary>
    /// Checks if a numeric value is positive and throws exception.
    /// </summary>
    public static void IsPositive<T>(T value, string message) where T : IComparable<T>
    {
        var zero = (T)Convert.ChangeType(0, typeof(T));
        if (value.CompareTo(zero) > 0)
            throw new DomainException(message);
    }

    #endregion
}
