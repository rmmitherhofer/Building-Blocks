using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swagger.Filters;

internal class AddOptionalHelperOperationFilter : IOperationFilter
{
    private const string SETTINGS_DEFAULT_HEADERS_NODE = "SwaggerSettings:DefaultHeaders:";

    private readonly bool _xClientIdRequired;
    private readonly bool _xForwardedForRequired;
    private readonly bool _xCorrelationIdRequired;
    private readonly bool _xUserAgentRequired;

    public AddOptionalHelperOperationFilter(IConfiguration configuration)
    {
        _xClientIdRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}X-Client-ID-Required"], out bool xClientIdRequired) ? xClientIdRequired : true;
        _xForwardedForRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}X-Forwarded-For-Required"], out bool xForwardedForRequired) ? xForwardedForRequired : false;
        _xCorrelationIdRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}X-Correlation-ID-Required"], out bool xCorrelationIdRequired) ? xCorrelationIdRequired : false;
        _xUserAgentRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}User-Agent-Required"], out bool xUserAgentRequired) ? xUserAgentRequired : false;
    }
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        var existingHeaders = operation.Parameters
            .Where(p => p.In == ParameterLocation.Header)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<OpenApiParameter> headers = [
            new(){
                Name = "X-Client-ID",
                In = ParameterLocation.Header,
                Required = _xClientIdRequired,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Client ID"
                },
                Description = "Client ID, used to identify the client application making the request."
            },
            new(){
                Name = "X-Forwarded-For",
                In = ParameterLocation.Header,
                Required = _xForwardedForRequired,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Forwarded IP address"
                },
                Description = "The original IP address of the client making the request."
            },
            new(){
                Name = "X-Correlation-ID",
                In = ParameterLocation.Header,
                Required = _xCorrelationIdRequired,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Correlation ID"
                },
                Description = "A unique identifier for tracing requests across systems."
            },
            new(){
                Name = "User-Agent",
                In = ParameterLocation.Header,
                Required = _xUserAgentRequired,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Description = "User Agent"
                },
                Description = "Information about the client application making the request."
            }
        ];

        foreach (var header in headers)
        {
            if (!existingHeaders.Contains(header.Name))            
                operation.Parameters.Add(header);            
        }
    }
}