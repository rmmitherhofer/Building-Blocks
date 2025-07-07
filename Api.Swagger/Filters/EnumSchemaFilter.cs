using Api.Swagger.Extensions;
using Api.Swagger.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.XPath;

namespace Api.Swagger.Filters;

/// <summary>
/// Schema filter to enrich enum types with their names and descriptions
/// in Swagger schema definitions.
/// </summary>
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
    private readonly HashSet<XPathNavigator> _xmlNavigators = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XEnumNamesSchemaFilter"/> class.
    /// </summary>
    /// <param name="options">Options to configure enum display behavior.</param>
    /// <param name="configureOptions">Optional additional configuration action.</param>
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
            {
                if (File.Exists(filePath))
                {
                    _xmlNavigators.Add(new XPathDocument(filePath).CreateNavigator());
                }
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies the schema filter to add enum names and descriptions
    /// as extensions to the OpenAPI schema.
    /// </summary>
    /// <param name="schema">The OpenAPI schema to modify.</param>
    /// <param name="context">The schema filter context providing type information.</param>
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

        // Handle generic types containing enums
        if (typeInfo.IsGenericType && !schema.Extensions.ContainsKey(_xEnumNamesAlias))
        {
            foreach (var genericArgumentType in typeInfo.GetGenericArguments())
            {
                if (genericArgumentType.IsEnum && schema.Properties?.Count > 0)
                {
                    foreach (var schemaProperty in schema.Properties)
                    {
                        var schemaPropertyValue = schemaProperty.Value;
                        var propertySchema = context.SchemaRepository.Schemas
                            .FirstOrDefault(s => schemaPropertyValue.AllOf
                                .FirstOrDefault(a => a.Reference.Id == s.Key) is not null).Value;

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
                var propertySchema = context.SchemaRepository.Schemas
                    .FirstOrDefault(s => schemaPropertyValue.AllOf
                        .FirstOrDefault(a => a.Reference.Id == s.Key) is not null).Value;

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
