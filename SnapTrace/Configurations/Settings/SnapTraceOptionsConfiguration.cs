namespace SnapTrace.Configurations.Settings;

/// <summary>
/// Global static configuration for SnapTrace options.
/// Provides centralized access to the <see cref="HandlerOptions"/> object.
/// </summary>
public static class SnapTraceOptionsConfiguration
{
    /// <summary>
    /// Static instance of the SnapTrace handler options.
    /// </summary>
    public static HandlerOptions Options { get; private set; } = new();
}
