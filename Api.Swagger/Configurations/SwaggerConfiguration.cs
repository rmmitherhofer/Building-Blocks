using Api.Swagger.Extensions;
using Api.Swagger.Filters;
using Api.Swagger.Middlewares;
using Api.Swagger.Options;
using Common.Extensions;
using Common.Logs.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Reflection;

namespace Api.Swagger.Configurations;

/// <summary>
/// Provides extension methods to configure Swagger and API versioning in the application.
/// </summary>
public static class SwaggerConfiguration
{
    private static string SETTINGS_FIXENUMSOPTIONS_NODE = "SwaggerSettings:FixEnumsOptions:";

    /// <summary>
    /// Adds and configures Swagger services, including custom filters and XML comments.
    /// Also configures API versioning.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application configuration for reading settings.</param>
    /// <returns>The IServiceCollection with Swagger and versioning services configured.</returns>
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

        services.Configure<SwaggerDefaultHeadersOptions>(configuration.GetSection("SwaggerSettings:DefaultHeaders"));

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
                        ConsoleLogExtensions.LogWarn($"{nameof(xmlPath)} not found at {xmlPath}, please verify if <GenerateDocumentationFile>true</GenerateDocumentationFile> is set in the project's .csproj file");
                    }
                }
            }
        });

        return services;
    }

    /// <summary>
    /// Adds API versioning and API explorer services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The IServiceCollection with versioning services configured.</returns>
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

    /// <summary>
    /// Adds and configures the Swagger middleware and Swagger UI in the application pipeline.
    /// Also adds the middleware to validate required headers based on configuration.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <param name="provider">The API version description provider.</param>
    /// <returns>The IApplicationBuilder with Swagger middleware configured.</returns>
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
                string isDeprecated = description.IsDeprecated ? " (Deprecated)" : string.Empty;

                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() + isDeprecated);
            }
        });

        app.TryUseMiddleware<RequestValidateHeadersMiddleware>();

        return app;
    }
}

