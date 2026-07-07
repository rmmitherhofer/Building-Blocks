using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Http.Attributes;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class MultipartFormDataExtensionsTests
{
    private enum Status
    {
        Active,
        Inactive
    }

    private sealed class NestedModel
    {
        public string? Code { get; set; }
    }

    private sealed class CustomNestedModel
    {
        [FormFieldName("code_value")]
        public string? Code { get; set; }
    }

    private sealed class ItemModel
    {
        public string? Value { get; set; }
    }

    private sealed class ItemMeta
    {
        public string? Code { get; set; }
    }

    private sealed class DeepItemModel
    {
        public ItemMeta? Meta { get; set; }
    }

    private sealed class ComplexModel
    {
        public string? Name { get; set; }

        [FormFieldName("user_id")]
        public Guid UserId { get; set; }

        public NestedModel? Meta { get; set; }

        public List<ItemModel>? Items { get; set; }

        public List<DeepItemModel>? DeepItems { get; set; }

        public IFormFile? Upload { get; set; }
    }

    private sealed class SimpleTypesModel
    {
        public int Count { get; set; }

        public Status Status { get; set; }

        public Guid CorrelationId { get; set; }
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it returns empty content")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_NullObject_ReturnsEmpty()
    {
        //Given
        object? model = null;

        //When
        var content = model.ToMultipartFormDataContent();

        //Then
        content.Should().BeEmpty("content should have no parts");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with nested data and a file, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it serializes properties with the default prefix")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_Object_SerializesWithDefaultPrefix()
    {
        //Given
        var model = new ComplexModel
        {
            Name = "John",
            UserId = Guid.Parse("d14c2f19-1d6f-4a5f-95db-36f5b21b1ac1"),
            Meta = new NestedModel { Code = "X1" },
            Items = new List<ItemModel>
            {
                new ItemModel { Value = "A" },
                new ItemModel { Value = "B" }
            },
            DeepItems = new List<DeepItemModel>
            {
                new DeepItemModel { Meta = new ItemMeta { Code = "D1" } }
            },
            Upload = CreateFormFile("upload.txt", "text/plain", "file-content")
        };

        //When
        var content = model.ToMultipartFormDataContent();
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[]
            {
                "file.Name",
                "file.user_id",
                "file.Meta.Code",
                "file.Items[0].Value",
                "file.Items[1].Value",
                "file.DeepItems[0].Meta.Code",
                "file.Upload"
            },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));

        var namePart = parts.Single(p => GetPartName(p) == "file.Name");
        var nameValue = await namePart.ReadAsStringAsync();
        nameValue.Should().Be("John", "content was '{0}'", nameValue);

        var userIdPart = parts.Single(p => GetPartName(p) == "file.user_id");
        var userIdValue = await userIdPart.ReadAsStringAsync();
        userIdValue.Should().Be("d14c2f19-1d6f-4a5f-95db-36f5b21b1ac1", "content was '{0}'", userIdValue);

        var uploadPart = parts.Single(p => GetPartName(p) == "file.Upload");
        GetFileName(uploadPart).Should().Be("upload.txt", "file part was '{0}'", GetFileName(uploadPart));
    }

    [Fact(DisplayName =
        "Given a file object, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it uses the provided field name")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_File_UsesCustomFieldName()
    {
        //Given
        var file = CreateFormFile("avatar.png", "image/png", "file-content");

        //When
        var content = file.ToMultipartFormDataContent("upload");
        var part = content.Single();

        //Then
        GetPartName(part).Should().Be("upload", "multipart name was '{0}'", GetPartName(part));
        GetFileName(part).Should().Be("avatar.png", "file part was '{0}'", GetFileName(part));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a list of files, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it indexes each file in the field name")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_FileList_IndexesFiles()
    {
        //Given
        var files = new List<IFormFile>
        {
            CreateFormFile("a.txt", "text/plain", "one"),
            CreateFormFile("b.txt", "text/plain", "two"),
            null!
        };

        //When
        var content = files.ToMultipartFormDataContent("files");
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[] { "files[0]", "files[1]" },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a list of simple values, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it indexes each item in the field name")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_ListOfSimpleValues_IndexesItems()
    {
        //Given
        var values = new[] { 1, 2 };

        //When
        var content = values.ToMultipartFormDataContent();
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[] { "file[0]", "file[1]" },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));

        var firstValue = await parts.Single(p => GetPartName(p) == "file[0]").ReadAsStringAsync();
        firstValue.Should().Be("1", "content was '{0}'", firstValue);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a string object, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it serializes the string as a single part")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_String_SerializesAsSinglePart()
    {
        //Given
        var value = "hello";

        //When
        var content = value.ToMultipartFormDataContent();
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[] { "file" },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));

        var body = await parts.Single().ReadAsStringAsync();
        body.Should().Be("hello", "content was '{0}'", body);
    }

    [Fact(DisplayName =
        "Given an object with an empty prefix, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it uses property names without the default prefix")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_Object_EmptyPrefix_UsesPropertyNames()
    {
        //Given
        var model = new
        {
            Name = "John",
            Meta = new CustomNestedModel { Code = "X1" }
        };

        //When
        var content = model.ToMultipartFormDataContent(string.Empty);
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[] { "Name", "Meta.code_value" },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with a null file property, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it does not include the file part")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_Object_NullFile_IsIgnored()
    {
        //Given
        var model = new ComplexModel
        {
            Name = "John",
            UserId = Guid.Parse("d14c2f19-1d6f-4a5f-95db-36f5b21b1ac1"),
            Meta = new NestedModel { Code = "X1" },
            Upload = null
        };

        //When
        var content = model.ToMultipartFormDataContent();
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().NotContain("file.Upload", "multipart names were '{0}'", string.Join(",", parts.Select(GetPartName)));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an object with simple types, " +
        "When ToMultipartFormDataContent is called, " +
        "Then it serializes values as string parts")]
    [Trait("Type", nameof(MultipartFormDataExtensions))]
    public async Task ToMultipartFormDataContent_Object_SimpleTypes_Serialized()
    {
        //Given
        var model = new SimpleTypesModel
        {
            Count = 3,
            Status = Status.Active,
            CorrelationId = Guid.Parse("4f2b1b9f-2b40-4f32-9b1a-4f3f0a2d6b3a")
        };

        //When
        var content = model.ToMultipartFormDataContent();
        var parts = content.ToList();

        //Then
        parts.Select(GetPartName).Should().BeEquivalentTo(
            new[] { "file.Count", "file.Status", "file.CorrelationId" },
            "multipart names were '{0}'",
            string.Join(",", parts.Select(GetPartName)));

        var countValue = await parts.Single(p => GetPartName(p) == "file.Count").ReadAsStringAsync();
        countValue.Should().Be("3", "content was '{0}'", countValue);

        var statusValue = await parts.Single(p => GetPartName(p) == "file.Status").ReadAsStringAsync();
        statusValue.Should().Be("Active", "content was '{0}'", statusValue);

        var idValue = await parts.Single(p => GetPartName(p) == "file.CorrelationId").ReadAsStringAsync();
        idValue.Should().Be("4f2b1b9f-2b40-4f32-9b1a-4f3f0a2d6b3a", "content was '{0}'", idValue);
    }

    private static string GetPartName(HttpContent part)
        => part.Headers.ContentDisposition?.Name?.Trim('"') ?? string.Empty;

    private static string GetFileName(HttpContent part)
        => part.Headers.ContentDisposition?.FileName?.Trim('"') ?? string.Empty;

    private static IFormFile CreateFormFile(string fileName, string contentType, string text)
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        var file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        return file;
    }
}
