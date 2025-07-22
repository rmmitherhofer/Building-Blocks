using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Zypher.Http.Attributes;

namespace Zypher.Http.Extensions;

/// <summary>
/// Extension methods to convert objects and files into <see cref="MultipartFormDataContent"/> 
/// for HTTP multipart/form-data requests, supporting <see cref="IFormFile"/> and collections,
/// and allowing custom form field names via <see cref="FormFieldNameAttribute"/>.
/// </summary>
public static class MultipartFormDataExtensions
{
    /// <summary>
    /// Converts an object to <see cref="MultipartFormDataContent"/> by serializing its public properties.
    /// Supports properties of types: <see cref="IFormFile"/>, <see cref="IEnumerable{IFormFile}"/>, 
    /// collections of strings, and simple types.
    /// Properties decorated with <see cref="FormFieldNameAttribute"/> will use the specified name as form field.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>A <see cref="MultipartFormDataContent"/> representing the serialized form data.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <paramref name="obj"/> is an <see cref="IFormFile"/> or <see cref="IEnumerable{IFormFile}"/>, 
    /// since these should be handled by specific overloads.
    /// </exception>
    public static MultipartFormDataContent ToMultipartFormDataContent(this object obj)
    {
        var content = new MultipartFormDataContent();

        if (obj == null)
            return content;

        var type = obj.GetType();

        if (obj is IFormFile || obj is IEnumerable<IFormFile>)
            throw new InvalidOperationException("Use the specific ToMultipartFormDataContent overload for IFormFile or IEnumerable<IFormFile>.");

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var propName = prop.GetCustomAttribute<FormFieldNameAttribute>()?.Name ?? prop.Name;
            var value = prop.GetValue(obj);

            if (value == null)
                continue;

            if (value is IFormFile propFile)
            {
                content.AddFileContent(propFile, propName);
            }
            else if (value is IEnumerable<IFormFile> propFiles)
            {
                foreach (var f in propFiles)
                    content.AddFileContent(f, propName);
            }
            else if (value is IEnumerable<string> stringEnumerable && !(value is string))
            {
                foreach (var s in stringEnumerable)
                {
                    content.Add(new StringContent(s ?? ""), propName);
                }
            }
            else
            {
                content.Add(new StringContent(value.ToString()), propName);
            }
        }
        return content;
    }

    /// <summary>
    /// Converts a single <see cref="IFormFile"/> into <see cref="MultipartFormDataContent"/> 
    /// with the specified form field name.
    /// </summary>
    /// <param name="file">The file to convert.</param>
    /// <param name="fieldName">The name of the form field for the file. Defaults to "file".</param>
    /// <returns>A <see cref="MultipartFormDataContent"/> containing the file content.</returns>

    public static MultipartFormDataContent ToMultipartFormDataContent(this IFormFile file, string fieldName = "file")
    {
        var content = new MultipartFormDataContent();
        content.AddFileContent(file, fieldName);
        return content;
    }

    /// <summary>
    /// Converts a collection of <see cref="IFormFile"/> into <see cref="MultipartFormDataContent"/> 
    /// with the specified form field name.
    /// </summary>
    /// <param name="files">The collection of files to convert.</param>
    /// <param name="fieldName">The name of the form field for the files. Defaults to "file".</param>
    /// <returns>A <see cref="MultipartFormDataContent"/> containing all files.</returns>

    public static MultipartFormDataContent ToMultipartFormDataContent(this IEnumerable<IFormFile> files, string fieldName = "file")
    {
        var content = new MultipartFormDataContent();
        foreach (var f in files)
        {
            content.AddFileContent(f, fieldName);
        }
        return content;
    }

    /// <summary>
    /// Adds an <see cref="IFormFile"/> as a <see cref="StreamContent"/> to a <see cref="MultipartFormDataContent"/>.
    /// Sets appropriate content disposition and content type headers.
    /// </summary>
    /// <param name="content">The multipart content to add the file to.</param>
    /// <param name="file">The file to add.</param>
    /// <param name="propName">The name of the form field.</param>

    private static void AddFileContent(this MultipartFormDataContent content, IFormFile file, string propName)
    {
        if (file is null) return;

        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{propName}\"",
            FileName = $"\"{file.FileName}\""
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

        content.Add(fileContent, propName, file.FileName);
    }
}
