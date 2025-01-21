using SnapTraceV2.Enums;

namespace SnapTraceV2.Models.Logger;

public class LogMessage
{
    public string Category { get; }
    public LogLevel LogLevel { get; }
    public string Message { get; }
    public DateTime DateTime { get; }
    public string? MemberType { get; }
    public string? MemberName { get; }
    public int LineNumber { get; }

    internal LogMessage(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Category, nameof(options.Category));

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Message, nameof(options.Message));

        Category = options.Category;
        LogLevel = options.LogLevel;
        Message = options.Message;
        MemberType = options.MemberType;
        MemberName = options.MemberName;
        LineNumber = options.LineNumber;
        DateTime = options.DateTime;
    }

    internal class CreateOptions
    {
        public string Category { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string? MemberType { get; set; }
        public string? MemberName { get; set; }
        public int LineNumber { get; set; }
        public DateTime DateTime { get; set; }

        public CreateOptions() => DateTime = DateTime.UtcNow;
    }

    internal LogMessage Clone()
        => new(new CreateOptions
        {
            Category = Category,
            LogLevel = LogLevel,
            Message = Message,
            MemberType = MemberType,
            MemberName = MemberName,
            LineNumber = LineNumber,
            DateTime = DateTime
        });
}
