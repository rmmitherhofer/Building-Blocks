using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class ValidationResponseTests
{
    [Fact(DisplayName =
        "Given notifications, " +
        "When ValidationResponse is created, " +
        "Then it copies the notifications")]
    [Trait("Type", nameof(ValidationResponse))]
    public async Task ValidationResponse_Ctor_CopiesNotifications()
    {
        //Given
        var notifications = new List<NotificationResponse>
        {
            new() { Key = "name", Value = "required" },
            new() { Key = "age", Value = "invalid" }
        };

        //When
        var response = new ValidationResponse(notifications);

        //Then
        response.Validations.Should().HaveCount(2);
        response.Validations.Should().ContainSingle(x => x.Key == "name");
        response.Validations.Should().ContainSingle(x => x.Key == "age");
        await Task.CompletedTask;
    }
}
