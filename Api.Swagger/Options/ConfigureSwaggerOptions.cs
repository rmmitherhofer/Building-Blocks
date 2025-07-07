using Common.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Api.Swagger.Options;

/// <summary>
/// Configures Swagger generation options for each API version.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigureSwaggerOptions"/>.
    /// </summary>
    /// <param name="provider">API version description provider.</param>
    /// <param name="configuration">Application configuration.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    /// <summary>
    /// Configures SwaggerGenOptions by registering a Swagger document for each API version.
    /// </summary>
    /// <param name="options">The SwaggerGenOptions to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            options.EnableAnnotations();
        }
    }

    /// <summary>
    /// Creates an <see cref="OpenApiInfo"/> instance for a given API version.
    /// Reads settings from configuration and appends a deprecation note if applicable.
    /// </summary>
    /// <param name="apiVersionDescription">The API version description.</param>
    /// <returns>An <see cref="OpenApiInfo"/> configured for the API version.</returns>
    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription apiVersionDescription)
    {
        const string SECTION = "SwaggerSettings";

        var title = _configuration.Get("Title", SECTION, true) ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "API";
        var version = _configuration.Get("Version", SECTION, true) ?? apiVersionDescription.ApiVersion.ToString();
        var description = _configuration.Get("Description", SECTION, true) ?? string.Empty;

        var info = new OpenApiInfo
        {
            Title = title,
            Version = version,
            Description = description
        };

        if (apiVersionDescription.IsDeprecated)
            info.Description += " is deprecated";

        return info;
    }
}
