using System.Globalization;

namespace Zypher.Logs.Extensions;

/// <summary>
/// Provides extension methods and helpers for <see cref="DateTime"/> operations and formatting,
/// focused on Brazilian Portuguese conventions.
/// </summary>
internal static class DateTimeExtensions
{
    /// <summary>
    /// Converts the date to a string formatted as "dd/MM/yyyy HH:mm:ss" using the "pt-BR" culture.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>The formatted date and time string.</returns>
    internal static string ToPtBrDateTime(this DateTime date)
        => date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"));   
}
