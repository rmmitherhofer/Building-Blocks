namespace Zypher.Http.Attributes;

/// <summary>
/// Specifies a custom query string parameter name for a property when generating query strings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueryStringPropertyAttribute"/> class with a specified query string key.
/// </remarks>
/// <param name="name">The custom name to use for the query string parameter.</param>
[AttributeUsage(AttributeTargets.Property)]
public class QueryStringPropertyAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the custom name to be used as the query string parameter.
    /// </summary>
    public string Name { get; } = name;
}