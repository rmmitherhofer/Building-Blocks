namespace SnapTraceV2.Models.Logger;

public class LogMessagesGroup
{
    public string Category { get; }
    public IEnumerable<LogMessage> Messages { get; }

    internal LogMessagesGroup(string category, IEnumerable<LogMessage> messages)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category, nameof(category));

        ArgumentNullException.ThrowIfNull(messages, nameof(IEnumerable<LogMessage>));

        Category = category;
        Messages = messages.ToList();
    }

    internal LogMessagesGroup Clone()
    {
        var messages = Messages.Select(p => p.Clone()).ToList();
        return new LogMessagesGroup(Category, messages);
    }
}
