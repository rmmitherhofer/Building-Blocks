using System.Text.RegularExpressions;

namespace Common.Http.Extensions;

public static partial class RouteTemplateExtensions
{
    [GeneratedRegex(@"\{([^{}]+)\}", RegexOptions.Compiled)]
    private static partial Regex Placeholder();

    /// <summary>
    /// Formats the route template by replacing placeholders with positional arguments.
    /// Placeholders like {id} will be replaced in order by args[0], args[1], etc.
    /// </summary>
    /// <param name="template">The route template containing placeholders.</param>
    /// <param name="args">Positional arguments to replace placeholders.</param>
    /// <returns>Formatted route string.</returns>
    public static string FormatRoute(this string template, params object[] args)
    {
        ArgumentException.ThrowIfNullOrEmpty(template, nameof(template));
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        var matches = Placeholder().Matches(template);

        for (int i = 0; i < matches.Count && i < args.Length; i++)
        {
            var key = matches[i].Groups[1].Value;
            var value = Uri.EscapeDataString(args[i]?.ToString() ?? string.Empty);
            template = template.Replace("{" + key + "}", value);
        }

        return template;
    }

    /// <summary>
    /// Formats the route template by replacing placeholders with properties from an object.
    /// The object's properties must match the placeholder names.
    /// </summary>
    /// <param name="template">The route template containing placeholders.</param>
    /// <param name="parameters">An object whose properties will replace placeholders.</param>
    /// <returns>Formatted route string.</returns>
    public static string FormatRoute(this string template, object parameters)
    {
        ArgumentException.ThrowIfNullOrEmpty(template, nameof(template));
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        var dict = parameters.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(parameters));

        return template.FormatRoute(dict);
    }

    /// <summary>
    /// Formats the route template by replacing placeholders with values from a dictionary.
    /// Keys are placeholder names and values are used to replace them.
    /// </summary>
    /// <param name="template">The route template containing placeholders.</param>
    /// <param name="parameters">Dictionary of placeholder names and values.</param>
    /// <returns>Formatted route string.</returns>
    public static string FormatRoute(this string template, IDictionary<string, object?> parameters)
    {
        ArgumentException.ThrowIfNullOrEmpty(template, nameof(template));
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        return Placeholder().Replace(template, match =>
        {
            var key = match.Groups[1].Value;

            return parameters.TryGetValue(key, out var value)
                ? Uri.EscapeDataString(value?.ToString() ?? string.Empty)
                : match.Value;
        });
    }
}
