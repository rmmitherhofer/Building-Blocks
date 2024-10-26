using Extensoes;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Swagger.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            options.EnableAnnotations();
        }
    }
    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription apiVersionDescription)
    {
        const string SECTION = "SwaggerSettings";

        var title = _configuration.Get("Title", SECTION, true) ?? Assembly.GetEntryAssembly().GetName().Name;
        var version = _configuration.Get("Version", SECTION, true) ?? apiVersionDescription.ApiVersion.ToString();
        var description = _configuration.Get("Description", SECTION, true) ?? "";

        OpenApiInfo info = new()
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
