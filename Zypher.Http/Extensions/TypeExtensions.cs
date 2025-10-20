using System.ComponentModel;

namespace Zypher.Http.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Type"/> objects.
/// </summary>
public static class TypeExtensions
{
    private static readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
    {
        typeof(bool),
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
        typeof(float), typeof(double),
        typeof(decimal),
        typeof(char)
    };

    private static readonly HashSet<Type> _otherSimpleTypes = new HashSet<Type>
    {
        typeof(string),
        typeof(Guid),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(DateOnly),
        typeof(TimeOnly)
    };

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> represents a simple type.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns>
    /// <see langword="true"/> if the type is considered simple; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// A type is considered simple if it meets one of the following criteria:
    /// <list type="bullet">
    /// <item><description>It is a primitive type (e.g., <see cref="int"/>, <see cref="bool"/>, <see cref="double"/>).</description></item>
    /// <item><description>It is one of the common simple types such as <see cref="string"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="DateOnly"/>, or <see cref="TimeOnly"/>.</description></item>
    /// <item><description>It is an <see cref="Enum"/>.</description></item>
    /// <item><description>It implements <see cref="IConvertible"/>.</description></item>
    /// <item><description>It can be converted from and to a <see cref="string"/> using a <see cref="TypeConverter"/>.</description></item>
    /// </list>
    /// This method also supports nullable types and recursively checks their underlying type.
    /// </remarks>
    public static bool IsSimpleType(this Type type)
    {
        if (type == null) return false;

        var underlying = Nullable.GetUnderlyingType(type);

        if (underlying != null) return IsSimpleType(underlying);

        if (_primitiveTypes.Contains(type)) return true;

        if (_otherSimpleTypes.Contains(type)) return true;

        if (type.IsEnum) return true;

        if (typeof(IConvertible).IsAssignableFrom(type)) return true;

        var conv = TypeDescriptor.GetConverter(type);

        return conv?.CanConvertFrom(typeof(string)) == true && conv.CanConvertTo(typeof(string));
    }
}