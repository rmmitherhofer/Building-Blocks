using System.Runtime.InteropServices;

namespace Zypher.Security.Jwt.Core.DefaultStore;

internal sealed class DefaultKeyStorageDirectories
{
    private static readonly Lazy<DirectoryInfo> _defaultDirectoruLazy = new(new Func<DirectoryInfo>(GetKeyStorageDirectoryImpl));

    private const string DataProtectionKeysFolderName = "DataProtectionKeys";

    public static DefaultKeyStorageDirectories Instace { get; } = new();

    public DirectoryInfo GetKeyStorageDirectory() => _defaultDirectoruLazy.Value;

    private static DirectoryInfo? GetKeyStorageDirectoryImpl()
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
        var home = Environment.GetEnvironmentVariable("HOME");

        DirectoryInfo? directoryInfo = null;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !string.IsNullOrEmpty(localAppData))
        {
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(folderPath);
        }
        else if (localAppData is not null)
        {
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(localAppData);
        }
        else if (userProfile is not null)
        {
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(Path.Combine(userProfile, "AppData", "Local"));
        }
        else if (home is not null)
        {
            directoryInfo = new(Path.Combine(home, ".aspnet", DataProtectionKeysFolderName));
        }
        else
        {
            if (string.IsNullOrEmpty(folderPath))
                return default;

            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(folderPath);
        }
        try
        {
            directoryInfo.Create();

            return directoryInfo;
        }
        catch (Exception)
        {
            return directoryInfo;
        }
    }


    public DirectoryInfo? GetKeyStorageDirectoryForAzureWebSites()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")))
        {
            string? environmentVariable = Environment.GetEnvironmentVariable("HOME");

            if (!string.IsNullOrEmpty(environmentVariable))
                return GetKeyStorageDirectoryFromBaseAppDataPath(environmentVariable);
        }
        return default;
    }
    private static DirectoryInfo GetKeyStorageDirectoryFromBaseAppDataPath(string baseAppDataPath)
        => new(Path.Combine(baseAppDataPath, "ASP.NET", DataProtectionKeysFolderName));
}
