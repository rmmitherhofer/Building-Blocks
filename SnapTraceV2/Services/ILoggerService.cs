using SnapTraceV2.Enums;
using SnapTraceV2.Json;

namespace SnapTraceV2.Services;

public interface ILoggerService
{
    void Log(LogLevel logLevel, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null);
    void Log(LogLevel logLevel, object json, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null);

    void Log(LogLevel logLevel, Exception ex,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null);

    void Log(LogLevel logLevel, Args.Args args, JsonSerializeOptions? options = null,
        [System.Runtime.CompilerServices.CallerMemberName] string? memberName = null,
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string? memberType = null);
}
