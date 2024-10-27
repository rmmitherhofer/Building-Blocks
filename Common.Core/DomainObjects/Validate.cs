using Common.Exceptions;
using System.Text.RegularExpressions;

namespace Common.Core.DomainObjects;

public static class Validate
{
    #region Equals
    public static void IsEquals(object object1, object object2, string message)
    {
        if (object1.Equals(object2))
            throw new DomainException(message);
    } 
    #endregion

    #region Different
    public static void IsDifferent(object object1, object object2, string message)
    {
        if (!object1.Equals(object2))
            throw new DomainException(message);
    }
    public static void IsDifferent(string pattern, string value, string message)
    {
        var regex = new Regex(pattern);
        if (!regex.IsMatch(value))
            throw new DomainException(message);
    }
    #endregion

    #region Null Or Empty
    public static void IsEmpty(string value, string message)
    {
        if (value == null || value.Trim().Length == 0)
            throw new DomainException(message);
    }
    public static void IsNull(object object1, string message)
    {
        if (object1 == null)
            throw new DomainException(message);
    } 
    #endregion

    #region MinMax

    public static void MinorOrMajor(string value, int min, int max, string message)
    {
        var length = value.Trim().Length;

        if (length < min || length > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(double value, double min, double max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(float value, float min, float max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(int value, int min, int max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(long value, long min, long max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(decimal value, decimal min, decimal max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    public static void MinorOrMajor(DateTime value, DateTime min, DateTime max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    #endregion

    #region Between
    public static void Between(string value, double min, double max, string message)
    {
        var length = value.Trim().Length;

        if (length >= min && length <= max)
            throw new DomainException(message);
    }
    public static void Between(double value, double min, double max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    public static void Between(float value, float min, float max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    public static void Between(int value, int min, int max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    public static void Between(long value, long min, long max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    public static void Between(decimal value, decimal min, decimal max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    public static void Between(DateTime value, DateTime min, DateTime max, string message)
    {
        if (value >= min && value <= max)
            throw new DomainException(message);
    }
    #endregion

    #region LessThan

    public static void LessThan(string value, int min, string message)
    {
        var length = value.Trim().Length;

        if (length < min)
            throw new DomainException(message);
    }
    public static void LessThan(long value, long min, string message)
    {
        if (value < min)
            throw new DomainException(message);
    }
    public static void LessThan(double value, double min, string message)
    {
        if (value < min)
            throw new DomainException(message);
    }
    public static void LessThan(decimal value, decimal min, string message)
    {
        if (value < min)
            throw new DomainException(message);
    }
    public static void LessThan(int value, int min, string message)
    {
        if (value < min)
            throw new DomainException(message);
    }
    public static void LessThan(DateTime value, DateTime min, string message)
    {
        if (value < min)
            throw new DomainException(message);
    }
    #endregion

    #region LessThanOrEqual
    public static void LessThanOrEqual(string value, int min, string message)
    {
        var length = value.Trim().Length;

        if (length <= min)
            throw new DomainException(message);
    }
    public static void LessThanOrEqual(long value, long min, string message)
    {
        if (value <= min)
            throw new DomainException(message);
    }
    public static void LessThanOrEqual(double value, double min, string message)
    {
        if (value <= min)
            throw new DomainException(message);
    }
    public static void LessThanOrEqual(decimal value, decimal min, string message)
    {
        if (value <= min)
            throw new DomainException(message);
    }
    public static void LessThanOrEqual(int value, int min, string message)
    {
        if (value <= min)
            throw new DomainException(message);
    }
    public static void LessThanOrEqual(DateTime value, DateTime min, string message)
    {
        if (value <= min)
            throw new DomainException(message);
    }
    #endregion

    #region GreaterThan

    public static void GreaterThan(string value, int max, string message)
    {
        var length = value.Trim().Length;

        if (length > max)
            throw new DomainException(message);
    }
    public static void GreaterThan(long value, long max, string message)
    {
        if (value > max)
            throw new DomainException(message);
    }
    public static void GreaterThan(double value, double max, string message)
    {
        if (value > max)
            throw new DomainException(message);
    }
    public static void GreaterThan(decimal value, decimal max, string message)
    {
        if (value > max)
            throw new DomainException(message);
    }
    public static void GreaterThan(int value, int max, string message)
    {
        if (value > max)
            throw new DomainException(message);
    }
    public static void GreaterThan(DateTime value, DateTime max, string message)
    {
        if (value > max)
            throw new DomainException(message);
    }
    #endregion

    #region GreaterThanOrEqual
    public static void GreaterThanOrEqual(string value, int max, string message)
    {
        var length = value.Trim().Length;

        if (length >= max)
            throw new DomainException(message);
    }
    public static void GreaterThanOrEqual(long value, long max, string message)
    {
        if (value >= max)
            throw new DomainException(message);
    }
    public static void GreaterThanOrEqual(double value, double max, string message)
    {
        if (value >= max)
            throw new DomainException(message);
    }
    public static void GreaterThanOrEqual(decimal value, decimal max, string message)
    {
        if (value >= max)
            throw new DomainException(message);
    }
    public static void GreaterThanOrEqual(int value, int max, string message)
    {
        if (value >= max)
            throw new DomainException(message);
    }
    public static void GreaterThanOrEqual(DateTime value, DateTime max, string message)
    {
        if (value >= max)
            throw new DomainException(message);
    }
    #endregion

    public static void IsFalse(bool value, string message)
    {
        if (!value)
            throw new DomainException(message);
    }
    public static void IsTrue(bool value, string message)
    {
        if (value)
            throw new DomainException(message);
    }
    public static void NotContains(string value, string[] list, string message)
    {
        if (!list.Contains(value.Trim()))
            throw new DomainException(message);
    }
    public static void Contains(string value, string[] list, string message)
    {
        if (list.Contains(value.Trim()))
            throw new DomainException(message);
    }
}
