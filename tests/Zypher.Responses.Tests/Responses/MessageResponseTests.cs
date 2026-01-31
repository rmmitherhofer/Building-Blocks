using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class MessageResponseTests
{
    private sealed class SampleMessageResponse : MessageResponse
    {
    }

    [Fact(DisplayName =
        "Given a message response, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(MessageResponse))]
    public async Task MessageResponse_Setters_PersistValues()
    {
        //Given
        var message = new SampleMessageResponse();
        var aggregateId = Guid.NewGuid();

        //When
        message.Type = "event";
        message.AggregateId = aggregateId;

        //Then
        message.Type.Should().Be("event");
        message.AggregateId.Should().Be(aggregateId);
        await Task.CompletedTask;
    }
}
