using Api.Swagger.Extensions;
using Api.Swagger.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.XPath;

namespace Api.Swagger.Filters;

/// <summary>
/// Parameter filter to add enum names and descriptions as Swagger extensions
/// to improve enum documentation in Swagger UI.
/// </summary>
internal class XEnumNamesParameterFilter : IParameterFilter
{
    #region Fields

    private readonly bool _includeXEnumDescriptions;
    private readonly bool _includeXEnumRemarks;
    private readonly string _xEnumNamesAlias;
    private readonly string _xEnumDescriptionsAlias;
    private readonly bool _includeDescriptionFromAttribute;
    private readonly string _newLine;
    private readonly DescriptionSources _descriptionSources;
    private readonly bool _applyFiler;
    private readonly HashSet<XPathNavigator> _xmlNavigators = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XEnumNamesParameterFilter"/> class.
    /// Reads configuration options to control behavior of the filter.
    /// </summary>
    /// <param name="options">The options for fixing enums configuration.</param>
    /// <param name="configureOptions">Optional action to further configure the options.</param>
    public XEnumNamesParameterFilter(IOptions<FixEnumsOptions> options, Action<FixEnumsOptions>? configureOptions = null)
    {
        if (options.Value is not null)
        {
            configureOptions?.Invoke(options.Value);
            _includeXEnumDescriptions = options.Value?.IncludeDescriptions ?? false;
            _includeXEnumRemarks = options.Value?.IncludeXEnumRemarks ?? false;
            _descriptionSources = options.Value?.DescriptionSource ?? DescriptionSources.DescriptionAttributes;
            _applyFiler = options.Value?.ApplyParameterFilter ?? false;
            _xEnumNamesAlias = options.Value?.XEnumNamesAlias;
            _xEnumDescriptionsAlias = options.Value?.XEnumDescriptionsAlias;
            _includeDescriptionFromAttribute = options.Value.IncludeDescriptions;
            _newLine = options.Value.NewLine;
            foreach (var filePath in options.Value?.IncludedXmlCommentsPaths ?? [])
            {
                if (File.Exists(filePath))
                    _xmlNavigators.Add(new XPathDocument(filePath).CreateNavigator());
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies the filter to the Swagger parameter, adding enum names and descriptions
    /// as extensions for better Swagger UI display.
    /// </summary>
    /// <param name="parameter">The OpenAPI parameter to update.</param>
    /// <param name="context">The parameter filter context providing type and schema information.</param>
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (!_applyFiler) return;

        var typeInfo = context.ParameterInfo?.ParameterType ?? context.PropertyInfo?.PropertyType;

        if (typeInfo is null) return;

        OpenApiArray enumsArray = new();
        OpenApiArray enumsDescriptionsArray = new();

        if (typeInfo.IsEnum)
        {
            var names = Enum
                .GetNames(typeInfo)
                .Select(name => (Enum.Parse(typeInfo, name), new OpenApiString(name)))
                .GroupBy(x => x.Item1)
                .Select(x => x.LastOrDefault().Item2)
                .ToList();

            enumsArray.AddRange(names);

            if (!parameter.Extensions.ContainsKey(_xEnumNamesAlias) && enumsArray.Any())
                parameter.Extensions.Add(_xEnumNamesAlias, enumsArray);

            if (_includeXEnumDescriptions)
            {
                enumsDescriptionsArray.AddRange(EnumTypeExtensions
                    .GetEnumValuesDescription(typeInfo, _descriptionSources, _xmlNavigators, _includeXEnumRemarks)
                    .GroupBy(x => x.EnumValue)
                    .Select(x => x.LastOrDefault().EnumDescription));

                if (!parameter.Extensions.ContainsKey(_xEnumDescriptionsAlias) && enumsDescriptionsArray.Any())
                    parameter.Extensions.Add(_xEnumDescriptionsAlias, enumsDescriptionsArray);
            }
        }
        else if (typeInfo.IsGenericType && !parameter.Extensions.ContainsKey(_xEnumNamesAlias))
        {
            foreach (var genericArgumentType in typeInfo.GetGenericArguments())
            {
                if (genericArgumentType.IsEnum)
                {
                    var names = Enum
                        .GetNames(genericArgumentType)
                        .Select(name => (Enum.Parse(genericArgumentType, name), new OpenApiString(name)))
                        .GroupBy(x => x.Item1)
                        .Select(x => x.LastOrDefault().Item2)
                        .ToList();

                    enumsArray.AddRange(names);

                    if (!parameter.Extensions.ContainsKey(_xEnumNamesAlias) && enumsArray.Any())
                        parameter.Extensions.Add(_xEnumNamesAlias, enumsArray);


                    if (_includeXEnumDescriptions)
                    {
                        enumsDescriptionsArray.AddRange(EnumTypeExtensions
                            .GetEnumValuesDescription(genericArgumentType, _descriptionSources, _xmlNavigators, _includeXEnumRemarks)
                            .GroupBy(x => x.EnumValue)
                            .Select(x => x.LastOrDefault().EnumDescription));

                        if (!parameter.Extensions.ContainsKey(_xEnumDescriptionsAlias) && enumsDescriptionsArray.Any())
                            parameter.Extensions.Add(_xEnumDescriptionsAlias, enumsDescriptionsArray);
                    }
                }
                else if (genericArgumentType.IsArray && genericArgumentType.GetElementType()?.IsEnum == true)
                {
                    var enumSchema = context.SchemaRepository.Schemas[parameter.Schema.Items.Items.Reference.Id];
                    var description = enumSchema.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeDescriptionFromAttribute, _newLine);
                    if (description is not null)
                    {
                        if (parameter.Description is null)
                            parameter.Description = description;
                        else if (!parameter.Description.Contains(description))
                            parameter.Description += description;
                    }
                }
            }
        }
        else if (typeInfo.IsArray && typeInfo.GetElementType()?.IsEnum == true)
        {
            var enumSchema = context.SchemaRepository.Schemas[parameter.Schema.Items.Reference.Id];
            var description = enumSchema.AddEnumValuesDescription(_xEnumNamesAlias, _xEnumDescriptionsAlias, _includeDescriptionFromAttribute, _newLine);

            if (description is not null)
            {
                if (parameter.Description is null)
                    parameter.Description = description;
                else if (!parameter.Description.Contains(description))
                    parameter.Description += description;
            }
        }
    }

    #endregion
}

