using SnapTrace.Enums;
using System.Reflection;

namespace SnapTrace.Configurations.Settings;

public class SnapTraceSettings
{
    public Guid ProjectId { get; set; }
    public ProjectType ProjectType { get; set; }
    public string Name { get; } = Assembly.GetEntryAssembly().GetName().Name;
    public bool TurnOnLog { get; set; }
    public int MaxResponseBodySizeInMb { get; set; } = 1;
    public SnapTraceHttpServiceSettings Service { get; set; }
}

public class SnapTraceHttpServiceSettings
{
    public string BaseAddress { get; set; }
    public SnapTraceHttpServiceEndPointsSettings EndPoints { get; set; }
}

public class SnapTraceHttpServiceEndPointsSettings
{
    public string Notify { get; set; }
}
