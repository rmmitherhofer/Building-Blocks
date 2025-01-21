using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.Factories;

internal class ReadStreamResult
{
    public string Content { get; set; }
    public TemporaryFile TemporaryFile { get; set; }
}
