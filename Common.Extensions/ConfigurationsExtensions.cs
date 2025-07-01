using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Extensoes;

public static class ConfigurationsExtensions
{
    public static IConfiguration Set(this IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configurationManager = (ConfigurationManager)configuration;

        configurationManager
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        return configurationManager;
    }

    public static string Get(this IConfiguration configuration, string key, string section = "AppSettings", bool nullable = false)
    {
        string setting = configuration.GetSection($"{(section == null ? string.Empty : section.EndsWith(":") ? section : section + ":")}{key.Replace(".", ":")}").Value;

        if (!string.IsNullOrEmpty(setting)) return setting;

        if (nullable) return setting;
        
        throw new ArgumentNullException(nameof(setting), $"{key} nao foi localizada no appsetings.json.");
    }
}
