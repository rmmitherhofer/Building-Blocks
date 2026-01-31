using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class NotificationResponseTests
{
    [Fact(DisplayName =
        "Given a notification response, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(NotificationResponse))]
    public async Task NotificationResponse_Setters_PersistValues()
    {
        //Given
        var response = new NotificationResponse();
        var id = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        //When
        response.Id = id;
        response.AggregateId = aggregateId;
        response.Type = "type";
        response.LogLevel = LogLevel.Error;
        response.Key = "key";
        response.Value = "value";
        response.Detail = "detail";
        response.Timestamp = timestamp;

        //Then
        response.Id.Should().Be(id);
        response.AggregateId.Should().Be(aggregateId);
        response.Type.Should().Be("type");
        response.LogLevel.Should().Be(LogLevel.Error);
        response.Key.Should().Be("key");
        response.Value.Should().Be("value");
        response.Detail.Should().Be("detail");
        response.Timestamp.Should().Be(timestamp);
        await Task.CompletedTask;
    }
}
