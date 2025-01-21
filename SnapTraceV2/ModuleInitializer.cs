using SnapTraceV2.Enums;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models;
using SnapTraceV2.Providers;
using System.Reflection;

namespace SnapTraceV2;

internal static class ModuleInitializer
{
    public static IReadInputStreamProvider ReadInputStreamProvider = new NullReadInputStreamProvider();
    public static void Init()
    {
        AssemblyName assembly = typeof(ModuleInitializer).Assembly.GetName();
        SnapTracePackage package = new(assembly.Name, assembly.Version);

        SnapTraceOptionsConfiguration.SnapTracePackages.Add(package);
    }

    public static void InitWhithProvider()
    {
        Init();

        SetReadInputStreamProvider();
    }

    private static void SetReadInputStreamProvider()
    {
        bool hasEnableBuffering = InternalHelper.WrapInTryCatch(() => HasEnableBuffering());

        if (hasEnableBuffering) ReadInputStreamProvider = new EnableBufferingReadInputStreamProvider();

        string type = ReadInputStreamProvider.GetType().Name;
        InternalLogHelper.Log($"ReadInputStreamProvider: {type}", LogLevel.Information);
    }

    private static bool HasEnableBuffering()
    {
        bool result = false;

        const string assemblyName = "Microsoft.AspNetCore.Http";
        const string typeName = "Microsoft.AspNetCore.Http.HttpRequestRewindExtensions";
        string assemblyQualifiedName = Assembly.CreateQualifiedName(assemblyName, typeName);

        Type? type = Type.GetType(assemblyQualifiedName, false);
        if (type != null)
        {
            var enableBuffering = type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "EnableBuffering");

            result = enableBuffering != null;
        }

        return result;
    }
}
