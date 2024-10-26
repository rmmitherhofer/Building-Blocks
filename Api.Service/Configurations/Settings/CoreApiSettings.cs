using SnapTrace.Configurations.Settings;

namespace Api.Service.Configurations.Settings;

public class CoreApiSettings
{
    public SnapTraceSettings SnapTraceSettings { get; private set; }

    public CoreApiSettings(SnapTraceSettings snapTraceSettings) => SnapTraceSettings = snapTraceSettings;
}
