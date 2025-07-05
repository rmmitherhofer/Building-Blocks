using System.Globalization;

namespace Common.Extensions;

/// <summary>
/// Provides extension methods and helpers for <see cref="DateTime"/> operations and formatting,
/// focused on Brazilian Portuguese conventions.
/// </summary>
public static class DateTimeHelpers
{
    /// <summary>
    /// Determines whether the specified date falls on a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> if the date is Saturday or Sunday; otherwise, <c>false</c>.</returns>
    public static bool IsWeekend(this DateTime date)
        => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Determines whether the specified date falls on a weekday (Monday to Friday).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> if the date is Monday through Friday; otherwise, <c>false</c>.</returns>
    public static bool IsWeekday(this DateTime date)
        => !date.IsWeekend();

    /// <summary>
    /// Determines whether the specified date is in the future relative to the current system time.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> if the date is greater than the current time; otherwise, <c>false</c>.</returns>
    public static bool IsFuture(this DateTime date)
        => date > DateTime.Now;

    /// <summary>
    /// Determines whether the specified date is in the past relative to the current system time.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><c>true</c> if the date is less than the current time; otherwise, <c>false</c>.</returns>
    public static bool IsPast(this DateTime date)
        => date < DateTime.Now;

    /// <summary>
    /// Converts the date to a string formatted as "dd/MM/yyyy" using the "pt-BR" culture.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>The formatted date string.</returns>
    public static string ToPtBrDate(this DateTime date)
        => date.ToString("dd/MM/yyyy", new CultureInfo("pt-BR"));

    /// <summary>
    /// Converts the date to a string formatted as "dd/MM/yyyy HH:mm:ss" using the "pt-BR" culture.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <returns>The formatted date and time string.</returns>
    public static string ToPtBrDateTime(this DateTime date)
        => date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"));

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the first day of the month for the specified date.
    /// </summary>
    /// <param name="date">The date whose month to evaluate.</param>
    /// <returns>The first day of the month.</returns>
    public static DateTime FirstDayOfMonth(this DateTime date)
        => new(date.Year, date.Month, 1);

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the last day of the month for the specified date.
    /// </summary>
    /// <param name="date">The date whose month to evaluate.</param>
    /// <returns>The last day of the month.</returns>
    public static DateTime LastDayOfMonth(this DateTime date)
        => new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

    /// <summary>
    /// Calculates the age in full years from the given birth date to the specified date or to today if none specified.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <param name="from">The date to calculate age from; if null, uses the current date.</param>
    /// <returns>The age in years.</returns>
    public static int AgeFrom(this DateTime birthDate, DateTime? from = null)
    {
        var today = from ?? DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age)) age--;

        return age;
    }
}
