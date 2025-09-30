using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Net.Http.Headers;
using System.Reflection;
using Zypher.Http.Attributes;

namespace Zypher.Http.Extensions;

/// <summary>
/// Provides extension methods to convert objects and files into <see cref="MultipartFormDataContent"/> 
/// for HTTP multipart/form-data requests. Supports <see cref="IFormFile"/>, collections, and 
/// allows custom form field names via <see cref="FormFieldNameAttribute"/>.
/// </summary>
public static class MultipartFormDataExtensions
{
    /// <summary>
    /// Converts an object into <see cref="MultipartFormDataContent"/> by serializing its public properties.
    /// Handles simple types, nested objects, collections, and files.
    /// Properties decorated with <see cref="FormFieldNameAttribute"/> will use the specified name as the form field.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>A <see cref="MultipartFormDataContent"/> representing the serialized object.</returns>
    public static MultipartFormDataContent ToMultipartFormDataContent(this object obj)
    {
        var content = new MultipartFormDataContent();
        AddObjectToContent(content, obj, prefix: string.Empty);
        return content;
    }

    /// <summary>
    /// Recursively adds the properties of an object into the given <see cref="MultipartFormDataContent"/>.
    /// Handles nested objects, collections, and files.
    /// </summary>
    /// <param name="content">The multipart content to populate.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="prefix">The prefix for property names (used for nested objects).</param>
    private static void AddObjectToContent(MultipartFormDataContent content, object obj, string prefix)
    {
        if (obj == null) return;

        var type = obj.GetType();

        if (obj is IFormFile singleFile)
        {
            content.AddFileContent(singleFile, prefix);
            return;
        }

        if (obj is IEnumerable<IFormFile> fileList)
        {
            int i = 0;
            foreach (var file in fileList)
                content.AddFileContent(file, $"{prefix}[{i++}]");
            return;
        }

        if (obj is IEnumerable enumerable && obj is not string)
        {
            int i = 0;
            foreach (var item in enumerable)
            {
                AddObjectToContent(content, item, $"{prefix}[{i++}]");
            }
            return;
        }

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var value = prop.GetValue(obj);
            if (value == null) continue;

            var customName = prop.GetCustomAttribute<FormFieldNameAttribute>()?.Name;
            var propName = string.IsNullOrEmpty(prefix)
                ? customName ?? prop.Name
                : $"{prefix}.{customName ?? prop.Name}";

            if (value is IFormFile file)
            {
                content.AddFileContent(file, propName);
            }
            else if (IsSimpleType(prop.PropertyType))
            {
                content.Add(new StringContent(value.ToString() ?? ""), propName);
            }
            else
            {
                AddObjectToContent(content, value, propName);
            }
        }
    }


    /// <summary>
    /// Adds a single <see cref="IFormFile"/> to the multipart content.
    /// Sets appropriate content disposition and content type headers.
    /// </summary>
    /// <param name="content">The multipart content to add the file to.</param>
    /// <param name="file">The file to add.</param>
    /// <param name="propName">The name of the form field.</param>
    private static void AddFileContent(this MultipartFormDataContent content, IFormFile file, string propName)
    {
        if (file == null) return;

        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{propName}\"",
            FileName = $"\"{file.FileName}\""
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

        content.Add(fileContent, propName, file.FileName);
    }


    /// <summary>
    /// Determines whether a type is a simple type (primitive, string, enum, decimal, DateTime, DateOnly, Guid, or nullable versions).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is simple; otherwise, false.</returns>
    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            type.IsEnum ||
            type.Equals(typeof(string)) ||
            type.Equals(typeof(decimal)) ||
            type.Equals(typeof(DateOnly)) ||
            type.Equals(typeof(DateTime)) ||
            type.Equals(typeof(Guid)) ||
            Nullable.GetUnderlyingType(type)?.IsEnum == true ||
            Nullable.GetUnderlyingType(type)?.IsPrimitive == true;
    }
}
