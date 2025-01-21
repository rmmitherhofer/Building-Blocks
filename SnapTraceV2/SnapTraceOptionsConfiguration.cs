using SnapTraceV2.Containeres;
using SnapTraceV2.Helpers;
using SnapTraceV2.Json;
using SnapTraceV2.Options;
using System.Diagnostics;

namespace SnapTraceV2;

public static class SnapTraceOptionsConfiguration
{
    internal static IJsonSerializer JsonSerializer { get; } = new SystemTextJsonSerializer();
    internal static SnapTracePackagesContainer SnapTracePackages { get; private set; } = new();

    public static LogListenersContainer Listeners { get; private set; } = new();
    public static HandlerOptions Options { get; private set; } = new();
    public static Action<string> InternalLog { get; set; } = (message) => Debug.WriteLine(message);

    static SnapTraceOptionsConfiguration() => InternalHelper.WrapInTryCatch(ModuleInitializer.Init);
}
