using SnapTraceV2.Models;

namespace SnapTraceV2.Containeres;

internal class SnapTracePackagesContainer
{
    private readonly List<SnapTracePackage> _list;
    public SnapTracePackagesContainer() => _list = [];

    public void Add(SnapTracePackage package)
    {
        ArgumentNullException.ThrowIfNull(package, nameof(SnapTracePackage));

        SnapTracePackage? existing = _list.FirstOrDefault(p => string.Compare(p.Name, package.Name, true) == 0);

        if (existing is not null)
        {
            if (existing.Version < package.Version)
            {
                _list.RemoveAll(p => string.Compare(p.Name, package.Name, true) == 0);
                _list.Add(package);
            }

            return;
        }

        _list.Add(package);
    }

    public SnapTracePackage? GetPrimaryPackage()
    {
        if (_list.Count == 0) return Constants.UnknownPackage;

        SnapTracePackage? package = _list.LastOrDefault(p => string.Compare(p.Name, "SnapTrace", true) is not 0);

        package ??= _list.LastOrDefault();

        return package;
    }

    internal IEnumerable<SnapTracePackage> GetAll() => [.. _list];
}
