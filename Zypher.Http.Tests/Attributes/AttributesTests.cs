using FluentAssertions;
using Xunit;
using Zypher.Http.Attributes;

namespace Zypher.Http.Tests.Attributes;

public class AttributesTests
{
    [Fact(DisplayName =
        "Given a QueryStringPropertyAttribute, " +
        "When it is created, " +
        "Then it exposes the configured name")]
    [Trait("Type", nameof(QueryStringPropertyAttribute))]
    public async Task QueryStringPropertyAttribute_ExposesName()
    {
        //Given
        var attribute = new QueryStringPropertyAttribute("user_id");

        //When
        var name = attribute.Name;

        //Then
        name.Should().Be("user_id");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a FormFieldNameAttribute, " +
        "When it is created, " +
        "Then it exposes the configured name")]
    [Trait("Type", nameof(FormFieldNameAttribute))]
    public async Task FormFieldNameAttribute_ExposesName()
    {
        //Given
        var attribute = new FormFieldNameAttribute("file_name");

        //When
        var name = attribute.Name;

        //Then
        name.Should().Be("file_name");
        await Task.CompletedTask;
    }
}
