using SnapTrace.Enums;
using System.Reflection;

namespace SnapTrace.Configurations.Settings;

public class SnapTraceSettings(ProjectType projectType, bool turnOnLog = true)
{
    public ProjectType ProjectType { get; } = projectType;
    public string Name { get; } = Assembly.GetCallingAssembly().FullName.Split(',')[0];
    public bool TurnOnLog { get; } = turnOnLog;
}
