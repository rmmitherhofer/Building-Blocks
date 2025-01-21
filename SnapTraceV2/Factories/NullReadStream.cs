namespace SnapTraceV2.Factories;

internal class NullReadStream : IReadStreamStrategy
{
    public ReadStreamResult Read() => new();
}
