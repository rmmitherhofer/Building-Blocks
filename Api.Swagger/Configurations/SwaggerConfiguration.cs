using Logs.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
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

        services.AddSwaggerGen(a =>
        {
            a.OperationFilter<SwaggerDefaultValues>();

            foreach (var xmlFile in xmlFiles)
            {
                if (!string.IsNullOrEmpty(xmlFile))
                {
                    string xmlPath = xmlFile.Replace(".dll", ".xml");
                    try
                    {
                        a.IncludeXmlComments(xmlPath);
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
            
            if(!Debugger.IsAttached)
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
