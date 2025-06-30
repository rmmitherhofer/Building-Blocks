
using Api.Swagger.Extensions;
using Logs.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swagger.Filters;
using Swagger.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Reflection;

namespace Swagger.Configurations;

public static class SwaggerConfiguration
{
    private static string SETTINGS_FIXENUMSOPTIONS_NODE = "SwaggerSettings:FixEnumsOptions:";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        var xmlFiles = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.ToLower().Contains(Assembly.GetEntryAssembly().GetName().Name.ToLower()
            .Replace(".api", "")))
            .Select(a => a.Location)
            .ToList();

        services.AddVersioningConfig();

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();

            c.OperationFilter<AddOptionalHelperOperationFilter>();

            c.AddEnumsWithValuesFixFilters(options =>
            {
                options.ApplyParameterFilter = bool.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}ApplyParameterFilter"], out bool applyParameterFilter) ? applyParameterFilter : true;
                options.ApplyDocumentFilter = bool.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}ApplyDocumentFilter"], out bool applyDocumentFilter) ? applyDocumentFilter : true;
                options.ApplySchemaFilter = bool.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}ApplySchemaFilter"], out bool applySchemaFilter) ? applySchemaFilter : true;
                options.DescriptionSource = Enum.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}DescriptionSource"], out DescriptionSources descriptionSource) ? descriptionSource : DescriptionSources.DescriptionAttributes;
                options.IncludeDescriptions = bool.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}IncludeDescriptions"], out bool includeDescriptions) ? includeDescriptions : false;
                options.IncludeXEnumRemarks = bool.TryParse(configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}IncludeXEnumRemarks"], out bool includeXEnumRemarks) ? includeXEnumRemarks : false;
                options.XEnumNamesAlias = configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}XEnumNamesAlias"] ?? "X-enumNames";
                options.XEnumDescriptionsAlias = configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}XEnumDescriptionsAlias"] ?? "x-enumDescriptions";
                options.NewLine = configuration[$"{SETTINGS_FIXENUMSOPTIONS_NODE}NewLine"] ?? "\n";
            });

            foreach (var xmlFile in xmlFiles)
            {
                if (!string.IsNullOrEmpty(xmlFile))
                {
                    string xmlPath = xmlFile.Replace(".dll", ".xml");
                    try
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                    catch
                    {
                        ConsoleLog.LogWarn($"{nameof(xmlPath)} não localizado em {xmlPath}, verifique se existe a tag <GenerateDocumentationFile>true</GenerateDocumentationFile> no .csproj do projeto");
                    }
                }
            }
        });
        return services;
    }

    private static IServiceCollection AddVersioningConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(provider);

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            string application = string.Empty;

            if (!Debugger.IsAttached)
                application = $"/{Assembly.GetEntryAssembly().GetName().Name.Replace(".Api", "").ToLower()}";

            foreach (var description in provider.ApiVersionDescriptions)
            {
                string isDeprecated = description.IsDeprecated ? " (Descontinuado)" : string.Empty;

                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() + isDeprecated);
            }
        });
        return app;
    }
}
