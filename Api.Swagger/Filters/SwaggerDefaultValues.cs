using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger.Filters;

/// <summary>
/// Applies default values and metadata from API descriptions to Swagger operation parameters.
/// This includes setting parameter descriptions, default values, required flags, and marking operations as deprecated.
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    /// <summary>
    /// Applies default values from the <see cref="ApiDescription"/> to the given <see cref="OpenApiOperation"/>.
    /// </summary>
    /// <param name="operation">The Swagger operation to modify.</param>
    /// <param name="context">The context providing API description and metadata.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters is null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
            var routeInfo = description.RouteInfo;

            parameter.Description ??= description.ModelMetadata?.Description;

            if (routeInfo is null)
                continue;

            if (parameter.In != ParameterLocation.Path && parameter.Schema.Default is null)
                parameter.Schema.Default = new OpenApiString(routeInfo.DefaultValue?.ToString());

            parameter.Required |= !routeInfo.IsOptional;
        }
    }
}
