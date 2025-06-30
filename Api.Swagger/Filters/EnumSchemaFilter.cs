using Api.Swagger.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swagger.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.XPath;

namespace Swagger.Filters;

internal class XEnumNamesSchemaFilter : ISchemaFilter
{
    #region Fields

    private readonly bool _includeXEnumDescriptions;
    private readonly bool _includeXEnumRemarks;
    private readonly string _xEnumNamesAlias;
    private readonly string _xEnumDescriptionsAlias;
    private readonly string _newLine;
    private readonly DescriptionSources _descriptionSources;
    private readonly bool _applyFiler;
    private readonly HashSet<XPathNavigator> _xmlNavigators = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options"><see cref="FixEnumsOptions"/>.</param>
    /// <param name="configureOptions">An <see cref="Action{FixEnumsOptions}"/> to configure options for filter.</param>
    public XEnumNamesSchemaFilter(IOptions<FixEnumsOptions> options, Action<FixEnumsOptions>? configureOptions = null)
    {
        if (options.Value is not null)
        {
            configureOptions?.Invoke(options.Value);

            _includeXEnumDescriptions = options.Value.IncludeDescriptions;
            _includeXEnumRemarks = options.Value.IncludeXEnumRemarks;
            _descriptionSources = options.Value.DescriptionSource;
            _applyFiler = options.Value.ApplySchemaFilter;
            _xEnumNamesAlias = options.Value.XEnumNamesAlias;
            _xEnumDescriptionsAlias = options.Value.XEnumDescriptionsAlias;
            _newLine = options.Value.NewLine;

            foreach (var filePath in options.Value.IncludedXmlCommentsPaths)
                if (File.Exists(filePath))
                    _xmlNavigators.Add(new XPathDocument(filePath).CreateNavigator());
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Apply the filter.
    /// </summary>
    /// <param name="schema"><see cref="OpenApiSchema"/>.</param>
    /// <param name="context"><see cref="SchemaFilterContext"/>.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!_applyFiler) return;

        var typeInfo = context.Type.GetTypeInfo();
        var enumsArray = new OpenApiArray();
        var enumsDescriptionsArray = new OpenApiArray();
        if (typeInfo.IsEnum)
        {
            var names = Enum
                .GetNames(context.Type)
                .Select(name => (Enum.Parse(context.Type, name), new OpenApiString(name)))
                .GroupBy(x => x.Item1)
                .Select(x => x.LastOrDefault().Item2)
                .ToList();

            enumsArray.AddRange(names);

            if (!schema.Extensions.ContainsKey(_xEnumNamesAlias) && enumsArray.Any())
                schema.Extensions.Add(_xEnumNamesAlias, enumsArray);

            if (_includeXEnumDescriptions)
            {
                enumsDescriptionsArray.AddRange(EnumTypeExtensions
                    .GetEnumValuesDescription(context.Type, _descriptionSources, _xmlNavigators, _includeXEnumRemarks)
                    .GroupBy(x => x.EnumValue)
                    .Select(x => x.LastOrDefault().EnumDescription));

                if (!schema.Extensions.ContainsKey(_xEnumDescriptionsAlias) && enumsDescriptionsArray.Any())
                    schema.Extensions.Add(_xEnumDescriptionsAlias, enumsDescriptionsArray);

            }
            return;
        }

        if (typeInfo.IsGenericType && !schema.Extensions.ContainsKey(_xEnumNamesAlias))
        {
            foreach (var genericArgumentType in typeInfo.GetGenericArguments())
            {
                if (genericArgumentType.IsEnum && schema.Properties?.Count > 0)
                {
                    foreach (var schemaProperty in schema.Properties)
                    {
                        var schemaPropertyValue = schemaProperty.Value;
                        var propertySchema = context.SchemaRepository.Schemas.FirstOrDefault(s => schemaPropertyValue.AllOf.FirstOrDefault(a => a.Reference.Id == s.Key) is not null).Value;
                        if (propertySchema is not null)
                        {
                            var names = Enum
                                .GetNames(genericArgumentType)
                                .Select(name => (Enum.Parse(genericArgumentType, name), new OpenApiString(name)))
                                .GroupBy(x => x.Item1)
                                .Select(x => x.LastOrDefault().Item2)
                                .ToList();

                            enumsArray.AddRange(names);

                            if (!schemaPropertyValue.Extensions.ContainsKey(_xEnumNamesAlias) && enumsArray.Any())
                                schemaPropertyValue.Extensions.Add(_xEnumNamesAlias, enumsArray);


                            if (_includeXEnumDescriptions)
                            {
                                enumsDescriptionsArray.AddRange(EnumTypeExtensions
                                    .GetEnumValuesDescription(genericArgumentType, _descriptionSources, _xmlNavigators, _includeXEnumRemarks)
                                    .GroupBy(x => x.EnumValue)
                                    .Select(x => x.LastOrDefault().EnumDescription));

                                if (!schemaPropertyValue.Extensions.ContainsKey(_xEnumDescriptionsAlias) && enumsDescriptionsArray.Any())
                                    schemaPropertyValue.Extensions.Add(_xEnumDescriptionsAlias, enumsDescriptionsArray);

                            }

                            var description = propertySchema.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeXEnumDescriptions, _newLine);
                            if (description is not null)
                            {
                                if (schemaPropertyValue.Description is null)
                                    schemaPropertyValue.Description = description;

                                else if (!schemaPropertyValue.Description.Contains(description))
                                    schemaPropertyValue.Description += description;

                            }
                        }
                    }
                }
            }
        }

        if (schema.Properties?.Count > 0)
        {
            foreach (var schemaProperty in schema.Properties)
            {
                var schemaPropertyValue = schemaProperty.Value;
                var propertySchema = context.SchemaRepository.Schemas.FirstOrDefault(s => schemaPropertyValue.AllOf.FirstOrDefault(a => a.Reference.Id == s.Key) is not null).Value;
                var description = propertySchema?.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeXEnumDescriptions, _newLine);
                if (description is not null)
                {
                    if (schemaPropertyValue.Description is null)
                        schemaPropertyValue.Description = description;

                    else if (!schemaPropertyValue.Description.Contains(description))
                        schemaPropertyValue.Description += description;
                }
            }
        }
    }

    #endregion
}

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
        _xForwardedForRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}X-Forwarded-For-Required"], out bool xForwardedForRequired) ? xForwardedForRequired : true;
        _xCorrelationIdRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}X-Correlation-ID-Required"], out bool xCorrelationIdRequired) ? xCorrelationIdRequired : true;
        _xUserAgentRequired = bool.TryParse(configuration[$"{SETTINGS_DEFAULT_HEADERS_NODE}User-Agent-Required"], out bool xUserAgentRequired) ? xUserAgentRequired : true;

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