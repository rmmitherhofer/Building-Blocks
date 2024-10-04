namespace Extensoes;

public static class DateTimeExtensions
{
    public static DateTime? MesAnoPtBRToDateTimeNulable(this string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;

        return date.MesAnoPtBRToDateTime();
    }
    public static DateTime MesAnoPtBRToDateTime(this string date)
    {
        try
        {
            var data = date.Split('/');

            int.TryParse(data[0], out int mes);
            int.TryParse(data[1][..4], out int ano);

            return new DateTime(ano, mes, 1, 0, 0, 0);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Formato de data invalida: {ex.Message}", ex);
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

            int.TryParse(data[0], out int dia);
            int.TryParse(data[1], out int mes);
            int.TryParse(data[2][..4], out int ano);

            return new DateTime(ano, mes, dia, 0, 0, 0);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Formato de data invalida: {ex.Message}", ex);
        }
    }

    public static DateTime DateTimePtBRToDateTime(this string dateTime)
    {
        try
        {
            var data = dateTime.Split('/');
            var horario = dateTime.Split(' ')[1];

            int.TryParse(data[0], out int dia);
            int.TryParse(data[1], out int mes);
            int.TryParse(data[2][..4], out int ano);
            int.TryParse(horario.Split(':')[0], out int hora);
            int.TryParse(horario.Split(':')[1], out int minuto);
            int.TryParse(horario.Split(':')[2], out int segundo);

            return new DateTime(ano, mes, dia, hora, minuto, segundo);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Formato de data invalida: {ex.Message}", ex);
        }
    }
}
