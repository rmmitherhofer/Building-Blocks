using Api.Swagger.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger.Filters;

/// <summary>
/// Operation filter that adds optional default and custom headers to Swagger documentation
/// based on the configured options.
/// </summary>
internal class AddOptionalHelperOperationFilter : IOperationFilter
{
    private readonly SwaggerDefaultHeadersOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddOptionalHelperOperationFilter"/> class
    /// with configured header options.
    /// </summary>
    /// <param name="options">The options containing default and additional headers configuration.</param>
    public AddOptionalHelperOperationFilter(IOptions<SwaggerDefaultHeadersOptions> options) => _options = options.Value;

    /// <summary>
    /// Applies the filter to add optional headers to the Swagger operation documentation.
    /// Headers are added only if enabled and if not already present in the operation.
    /// </summary>
    /// <param name="operation">The Swagger operation to modify.</param>
    /// <param name="context">The context of the operation filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        var existingHeaders = operation.Parameters
            .Where(p => p.In == ParameterLocation.Header)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var headers = new List<OpenApiParameter>();

        void AddHeader(string name, HeaderOption option)
        {
            if (option.Enabled)
            {
                headers.Add(new OpenApiParameter
                {
                    Name = name,
                    In = ParameterLocation.Header,
                    Required = option.Required,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = option.Description
                });
            }
        }

        AddHeader("X-Client-ID", _options.XClientId);
        AddHeader("X-Forwarded-For", _options.XForwardedFor);
        AddHeader("X-Correlation-ID", _options.XCorrelationId);
        AddHeader("User-Agent", _options.UserAgent);

        foreach (var customHeader in _options.AdditionalHeaders)
        {
            if (!string.IsNullOrWhiteSpace(customHeader.Name))
            {
                headers.Add(new OpenApiParameter
                {
                    Name = customHeader.Name,
                    In = ParameterLocation.Header,
                    Required = customHeader.Required,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = customHeader.Description
                });
            }
        }

        foreach (var header in headers)
        {
            if (!existingHeaders.Contains(header.Name))
                operation.Parameters.Add(header);
        }
    }
}
