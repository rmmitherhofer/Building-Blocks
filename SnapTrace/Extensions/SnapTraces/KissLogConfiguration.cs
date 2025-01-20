using SnapTrace.Interfaces.Json;
using SnapTrace.Json;
using System.Diagnostics;

namespace SnapTrace
{
    public static class KissLogConfiguration
    {
        internal static IJsonSerializer JsonSerializer { get; } = new SystemTextJsonSerializer();
        internal static KissLogPackagesContainer KissLogPackages { get; private set; } = new KissLogPackagesContainer();

        public static LogListenersContainer Listeners { get; private set; } = new LogListenersContainer();
        public static Options Options { get; private set; } = new Options();
        public static Action<string> InternalLog { get; set; } = (message) => Debug.WriteLine(message);

        static KissLogConfiguration()
        {
            InternalHelpers.WrapInTryCatch(() =>
            {
                ModuleInitializer.Init();
            });
        }
    }
}
