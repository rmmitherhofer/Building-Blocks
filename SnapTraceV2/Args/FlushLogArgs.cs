using SnapTraceV2.Models;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.Args;

public class FlushLogArgs
{
    public HttpProperties HttpProperties { get; }
    public IEnumerable<LogMessagesGroup> MessagesGroups { get; private set; }
    public IEnumerable<CapturedException> Exceptions { get; }
    public IEnumerable<LoggedFile> Files { get; private set; }
    public IEnumerable<KeyValuePair<string, object>> CustomProperties { get; }
    public bool IsCreatedByHttpRequest { get; }

    internal FlushLogArgs(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));
        ArgumentNullException.ThrowIfNull(options.HttpProperties, nameof(options.HttpProperties));
        ArgumentNullException.ThrowIfNull(options.HttpProperties.Response, nameof(options.HttpProperties.Response));

        HttpProperties = options.HttpProperties;
        MessagesGroups = options.MessagesGroups ?? [];
        Exceptions = options.Exceptions ?? [];
        Files = options.Files ?? [];
        CustomProperties = options.CustomProperties ?? [];
        IsCreatedByHttpRequest = options.IsCreatedByHttpRequest;
    }

    internal void SetFiles(IEnumerable<LoggedFile> files)
    {
        ArgumentNullException.ThrowIfNull(files, nameof(IEnumerable<LoggedFile>));

        Files = files.ToList();
    }

    internal void SetMessagesGroups(IEnumerable<LogMessagesGroup> messages)
    {
        ArgumentNullException.ThrowIfNull(messages, nameof(IEnumerable<LogMessagesGroup>));

        MessagesGroups = messages.ToList();
    }

    public class CreateOptions
    {
        public HttpProperties HttpProperties { get; set; }
        public List<LogMessagesGroup> MessagesGroups { get; set; }
        public List<CapturedException> Exceptions { get; set; }
        public List<LoggedFile> Files { get; set; }
        public List<KeyValuePair<string, object>> CustomProperties { get; set; }
        public bool IsCreatedByHttpRequest { get; set; }
    }

    internal FlushLogArgs Clone()
        => new(new CreateOptions
        {
            HttpProperties = HttpProperties.Clone(),
            MessagesGroups = MessagesGroups.Select(p => p.Clone()).ToList(),
            Exceptions = Exceptions.ToList(),
            Files = Files.Select(p => p.Clone()).ToList(),
            CustomProperties = CustomProperties.ToList(),
            IsCreatedByHttpRequest = IsCreatedByHttpRequest
        });
}
