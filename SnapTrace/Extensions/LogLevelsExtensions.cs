using SnapTrace.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SnapTrace.Extensions;

internal static class LogLevelsExtensions
{
    public static LogEntry Member(this LogEntry entry, [CallerMemberName] string? memberName = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string? memberType = null)
    {
        if (entry is null) return entry;

        var stackTrace = new StackTrace(true);
        StackFrame? userFrame = null;

        // Ajuste o namespace abaixo para o seu projeto
        var userNamespace = "Solucao.RH"; // ou outro prefixo do seu código

        foreach (var frame in stackTrace.GetFrames() ?? Array.Empty<StackFrame>())
        {
            var method = frame.GetMethod();
            if (method?.DeclaringType?.Namespace != null &&
                method.DeclaringType.Namespace.StartsWith(userNamespace))
            {
                userFrame = frame;
                break;
            }
        }

        if (userFrame != null)
        {
            entry.MemberName = userFrame.GetMethod()?.Name ?? memberName;
            entry.MemberType = userFrame.GetMethod()?.DeclaringType?.FullName ?? memberType;
            entry.LineNumber = userFrame.GetFileLineNumber() != 0 ? userFrame.GetFileLineNumber() : lineNumber;
        }
        else
        {
            entry.MemberName = memberName;
            entry.MemberType = memberType;
            entry.LineNumber = lineNumber;
        }

        return entry;
    }
}
