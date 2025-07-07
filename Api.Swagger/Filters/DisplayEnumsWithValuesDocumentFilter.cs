using Api.Swagger.Extensions;
using Api.Swagger.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger.Filters;

/// <summary>
/// Document filter to enhance enum types in Swagger documentation by appending
/// their values and descriptions.
/// </summary>
internal class DisplayEnumsWithValuesDocumentFilter : IDocumentFilter
{
    #region Fields

    private readonly bool _applyFiler;
    private readonly bool _includeDescriptionFromAttribute;
    private readonly string _xEnumNamesAlias;
    private readonly string _xEnumDescriptionsAlias;
    private readonly string _newLine;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayEnumsWithValuesDocumentFilter"/> class.
    /// </summary>
    /// <param name="options">Configuration options for enum display.</param>
    /// <param name="configureOptions">Optional action to configure the options.</param>
    public DisplayEnumsWithValuesDocumentFilter(IOptions<FixEnumsOptions> options, Action<FixEnumsOptions>? configureOptions = null)
    {
        if (options.Value is not null)
        {
            configureOptions?.Invoke(options.Value);
            _includeDescriptionFromAttribute = options.Value.IncludeDescriptions;
            _applyFiler = options.Value.ApplyDocumentFilter;
            _xEnumNamesAlias = options.Value.XEnumNamesAlias;
            _xEnumDescriptionsAlias = options.Value.XEnumDescriptionsAlias;
            _newLine = options.Value.NewLine;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies the document filter to enrich enum schemas and parameters with
    /// values and descriptions in the generated Swagger documentation.
    /// </summary>
    /// <param name="openApiDoc">The Swagger/OpenAPI document.</param>
    /// <param name="context">The filter context.</param>
    public void Apply(OpenApiDocument openApiDoc, DocumentFilterContext context)
    {
        if (!_applyFiler) return;

        // Update schemas to append enum values and descriptions
        foreach (var schemaDictionaryItem in openApiDoc.Components.Schemas)
        {
            var schema = schemaDictionaryItem.Value;
            var description = schema.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeDescriptionFromAttribute, _newLine);
            if (description is not null)
            {
                if (schema.Description is null)
                    schema.Description = description;
                else if (!schema.Description.Contains(description))
                    schema.Description += description;
            }
        }

        if (openApiDoc.Paths.Count <= 0) return;

        // Update parameters schemas in operations
        foreach (var parameter in openApiDoc.Paths.Values.SelectMany(v => v.Operations).SelectMany(op => op.Value.Parameters))
        {
            OpenApiSchema schema = null;

            if (parameter.Schema?.Reference is null)
            {
                if (parameter.Schema?.AllOf?.Count > 0)
                    schema = context.SchemaRepository.Schemas.FirstOrDefault(s => parameter.Schema.AllOf.FirstOrDefault(a => a.Reference.Id == s.Key) != null).Value;
                else
                    continue;
            }
            else
            {
                var componentReference = parameter.Schema?.Reference?.Id;

                if (!string.IsNullOrWhiteSpace(componentReference))
                    schema = openApiDoc.Components.Schemas[componentReference];
            }

            if (schema is not null)
            {
                var description = schema.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeDescriptionFromAttribute, _newLine);
                if (description is not null)
                {
                    if (parameter.Description is null)
                        parameter.Description = description;
                    else if (!parameter.Description.Contains(description))
                        parameter.Description += description;
                }
            }
        }

        foreach (var operation in openApiDoc.Paths.Values.SelectMany(v => v.Operations))
        {
            var requestBodyContents = operation.Value.RequestBody?.Content;
            if (requestBodyContents is not null)
            {
                foreach (var requestBodyContent in requestBodyContents)
                {
                    if (requestBodyContent.Value.Schema?.Reference?.Id is not null)
                    {
                        var schema = context.SchemaRepository.Schemas[requestBodyContent.Value.Schema.Reference.Id];
                        if (schema is not null)
                        {
                            requestBodyContent.Value.Schema.Description = schema.Description;
                            requestBodyContent.Value.Schema.Extensions = schema.Extensions;
                        }
                    }
                }
            }
        }
    }

    #endregion
}
