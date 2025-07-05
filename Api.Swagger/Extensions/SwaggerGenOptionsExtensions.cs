using Api.Swagger.Filters;
using Api.Swagger.Options;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger.Extensions;

/// <summary>
/// Provides extension methods to configure SwaggerGen options for enhanced enum documentation.
/// </summary>
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Adds filters to SwaggerGen options that improve enum handling by including enum names and values in the generated Swagger documentation.
    /// This method applies schema, parameter, and document filters related to enums, with optional configuration.
    /// </summary>
    /// <param name="swaggerGenOptions">The SwaggerGen options instance to configure.</param>
    /// <param name="configureOptions">An optional action to configure the <see cref="FixEnumsOptions"/> settings.</param>
    /// <returns>The updated <see cref="SwaggerGenOptions"/> instance for fluent chaining.</returns>
    public static SwaggerGenOptions AddEnumsWithValuesFixFilters(this SwaggerGenOptions swaggerGenOptions, Action<FixEnumsOptions>? configureOptions = null)
    {
        void EmptyAction(FixEnumsOptions x) { }

        swaggerGenOptions.SchemaFilter<XEnumNamesSchemaFilter>(configureOptions ?? EmptyAction);
        swaggerGenOptions.ParameterFilter<XEnumNamesParameterFilter>(configureOptions ?? EmptyAction);
        swaggerGenOptions.DocumentFilter<DisplayEnumsWithValuesDocumentFilter>();
        return swaggerGenOptions;
    }
}
