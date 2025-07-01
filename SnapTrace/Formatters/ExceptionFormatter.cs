using SnapTrace.Configurations.Settings;
using System.Text;

namespace SnapTrace.Formatters;

public class ExceptionFormatter
{
    private const string ExceptionLoggedKey = "SnapTrace-ExceptionLogged";

    public string Format(Exception ex)
    {
        StringBuilder sb = new();

        FormatException(ex, sb);

        if (SnapTraceOptionsConfiguration.Options.Handlers.AppendExceptionDetails is not null)
        {
            string append = SnapTraceOptionsConfiguration.Options.Handlers.AppendExceptionDetails.Invoke(ex);
            if (!string.IsNullOrWhiteSpace(append))
            {
                sb.AppendLine();
                sb.AppendLine(append);
            }
        }

        return sb.ToString().Trim();
    }

    private void FormatException(Exception ex, StringBuilder sb, string? header = null)
    {
        string id = $"{ExceptionLoggedKey}";

        bool alreadyLogged = ex.Data.Contains(id);

        if (alreadyLogged) return;

        ex.Data.Add(id, true);

        if (!string.IsNullOrEmpty(header)) sb.AppendLine(header);

        sb.AppendLine(ex.ToString());

        Exception? innerException = ex.InnerException;

        while (innerException is not null)
        {
            if (!innerException.Data.Contains(id))
                innerException.Data.Add(id, true);

            innerException = innerException.InnerException;
        }

        Exception baseException = ex.GetBaseException();

        if (baseException is not null)
            FormatException(baseException, sb, "Base Exception:");
    }
}
