namespace Extensoes;

public static class DateTimeExtensions
{
    public static DateTime? YearMonthPtBRToDateTimeNulable(this string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;

        return date.YearMonthPtBRToDateTime();
    }
    public static DateTime YearMonthPtBRToDateTime(this string date)
    {
        try
        {
            var data = date.Split('/');

            int.TryParse(data[0], out int month);
            int.TryParse(data[1][..4], out int year);

            return new DateTime(year, month, 1, 0, 0, 0);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Invalid date format: {ex.Message}", ex);
        }
    }
    public static DateTime? DatePtBRToDateTimeNulable(this string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;

        return date.DatePtBRToDateTime();
    }
    public static DateTime DatePtBRToDateTime(this string date)
    {
        try
        {
            var data = date.Split('/');

            int.TryParse(data[0], out int day);
            int.TryParse(data[1], out int month);
            int.TryParse(data[2][..4], out int year);

            return new DateTime(year, month, day, 0, 0, 0);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Invalid date format: {ex.Message}", ex);
        }
    }

    public static DateTime DateTimePtBRToDateTime(this string dateTime)
    {
        try
        {
            var data = dateTime.Split('/');
            var horario = dateTime.Split(' ')[1];

            int.TryParse(data[0], out int day);
            int.TryParse(data[1], out int month);
            int.TryParse(data[2][..4], out int year);
            int.TryParse(horario.Split(':')[0], out int hour);
            int.TryParse(horario.Split(':')[1], out int minute);
            int.TryParse(horario.Split(':')[2], out int second);

            return new DateTime(year, month, day, hour, minute, second);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Invalid date format: {ex.Message}", ex);
        }
    }
}
