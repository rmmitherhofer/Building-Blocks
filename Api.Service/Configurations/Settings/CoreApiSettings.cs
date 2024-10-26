using SnapTrace.Configurations.Settings;

namespace Api.Service.Configurations.Settings;

public class CoreApiSettings
{
    public LogMonitorSettings LogMonitorSettings { get; private set; }

    public CoreApiSettings(LogMonitorSettings logMonitorSettings)
    {
        LogMonitorSettings = logMonitorSettings;
    }
}
