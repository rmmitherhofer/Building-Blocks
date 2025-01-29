using Microsoft.Extensions.DependencyInjection;
using Swagger.Filters;
using Swagger.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger.Extensions;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions AddEnumsWithValuesFixFilters(this SwaggerGenOptions swaggerGenOptions, Action<FixEnumsOptions>? configureOptions = null)
    {
        void EmptyAction(FixEnumsOptions x) { }

        swaggerGenOptions.SchemaFilter<XEnumNamesSchemaFilter>(configureOptions ?? EmptyAction);
        swaggerGenOptions.ParameterFilter<XEnumNamesParameterFilter>(configureOptions ?? EmptyAction);
        swaggerGenOptions.DocumentFilter<DisplayEnumsWithValuesDocumentFilter>();
        return swaggerGenOptions;
    }
}
