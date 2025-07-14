using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Zypher.Extensions.Core;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/> configuration setup and retrieval.
/// </summary>
public static class ConfigurationsExtensions
{
    /// <summary>
    /// Configures the <see cref="IConfiguration"/> instance with JSON files and environment variables
    /// based on the given <see cref="IWebHostEnvironment"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to configure. Must be a <see cref="ConfigurationManager"/>.</param>
    /// <param name="environment">The hosting environment used to determine environment-specific settings.</param>
    /// <returns>The configured <see cref="IConfiguration"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> or <paramref name="environment"/> is null.</exception>
    /// <exception cref="InvalidCastException">Thrown if the <paramref name="configuration"/> instance is not a <see cref="ConfigurationManager"/>.</exception>
    public static IConfiguration Set(this IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(environment, nameof(environment));

        if (configuration is not ConfigurationManager configManager)
            throw new InvalidCastException("IConfiguration instance must be of type ConfigurationManager.");

        configManager
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return configManager;
    }

    /// <summary>
    /// Gets a configuration value by key, optionally within a specific section.
    /// Throws if the key is not found unless <paramref name="nullable"/> is true.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="key">The key of the configuration value (dot-separated keys are converted to colon format).</param>
    /// <param name="section">The optional section name to prepend to the key (default is "AppSettings").</param>
    /// <param name="nullable">Whether to allow returning null or empty if the key is not found (default is false).</param>
    /// <returns>The configuration value as a string, or null if allowed and not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> or <paramref name="key"/> is null or empty, or if the value is not found and nullable is false.</exception>
    public static string Get(this IConfiguration configuration, string key, string section = "AppSettings", bool nullable = false)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        var path = string.IsNullOrEmpty(section)
            ? key.Replace(".", ":")
            : $"{section.TrimEnd(':')}:{key.Replace(".", ":")}";

        var value = configuration[path];

        if (!string.IsNullOrWhiteSpace(value)) return value;

        if (nullable) return value;

        throw new ArgumentNullException(nameof(value), $"Configuration key '{path}' was not found in the configuration sources.");
    }
}
