using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Notifications.Messages;

namespace Zypher.Notifications.Tests.Messages;

public class MessageTests
{
    private sealed class SampleMessage : Message
    {
        public SampleMessage()
        {
        }

        public SampleMessage(string type) : base(type)
        {
        }
    }

    [Fact(DisplayName =
        "Given a derived message, " +
        "When it is created without a type, " +
        "Then it uses the class name")]
    [Trait("Type", nameof(Message))]
    public async Task Message_DefaultConstructor_UsesClassName()
    {
        //Given
        var message = new SampleMessage();

        //When
        var type = message.Type;

        //Then
        type.Should().Be(nameof(SampleMessage));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a custom type, " +
        "When the message is created, " +
        "Then it uses the provided type")]
    [Trait("Type", nameof(Message))]
    public async Task Message_CustomType_UsesProvidedType()
    {
        //Given
        var message = new SampleMessage("custom-type");

        //When
        var type = message.Type;

        //Then
        type.Should().Be("custom-type");
        await Task.CompletedTask;
    }
}
