using System;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Zypher.Json;

namespace Zypher.Json.Tests.Extensions;

public class JsonFileExtensionsTests
{
    private sealed class Sample
    {
        public string? Name { get; set; }
    }

    [Fact(DisplayName =
        "Given a file path and object, " +
        "When ObjectToJsonFile is called, " +
        "Then it writes the JSON file")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task ObjectToJsonFile_WritesFile()
    {
        //Given
        var temp = Path.GetTempFileName();
        var data = new Sample { Name = "John" };

        try
        {
            //When
            JsonFileExtensions.ObjectToJsonFile(temp, data);
            var content = File.ReadAllText(temp);

            //Then
            content.Should().Contain("\"name\"");
        }
        finally
        {
            File.Delete(temp);
        }

        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid path, " +
        "When ObjectToJsonFile is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task ObjectToJsonFile_InvalidPath_Throws()
    {
        //Given
        var data = new Sample { Name = "John" };

        //When
        Action action = () => JsonFileExtensions.ObjectToJsonFile(string.Empty, data);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null object, " +
        "When ObjectToJsonFile is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task ObjectToJsonFile_NullData_Throws()
    {
        //Given
        string path = Path.GetTempFileName();

        try
        {
            //When
            Action action = () => JsonFileExtensions.ObjectToJsonFile<object>(path, null!);

            //Then
            action.Should().Throw<ArgumentNullException>();
        }
        finally
        {
            File.Delete(path);
        }

        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JSON file, " +
        "When JsonToObject is called, " +
        "Then it reads and deserializes the file")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task JsonToObject_ReadsFile()
    {
        //Given
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, "{\"name\":\"John\"}");

        try
        {
            //When
            var result = JsonFileExtensions.JsonToObject<Sample>(temp);

            //Then
            result.Should().NotBeNull();
            result!.Name.Should().Be("John");
        }
        finally
        {
            File.Delete(temp);
        }

        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a missing file, " +
        "When JsonToObject is called, " +
        "Then it throws FileNotFoundException")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task JsonToObject_FileMissing_Throws()
    {
        //Given
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");

        //When
        Action action = () => JsonFileExtensions.JsonToObject<Sample>(path);

        //Then
        action.Should().Throw<FileNotFoundException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty path, " +
        "When JsonToObject is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(JsonFileExtensions))]
    public async Task JsonToObject_InvalidPath_Throws()
    {
        //Given
        var path = string.Empty;

        //When
        Action action = () => JsonFileExtensions.JsonToObject<Sample>(path);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }
}
