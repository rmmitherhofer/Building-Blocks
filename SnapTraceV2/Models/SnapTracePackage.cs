namespace SnapTraceV2.Models;

internal class SnapTracePackage
{
    public string Name { get; }
    public Version Version { get; }

    public SnapTracePackage(string name, Version version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        ArgumentNullException.ThrowIfNull(version, nameof(System.Version));

        Name = name;
        Version = version;
    }
}
