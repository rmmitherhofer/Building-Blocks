using Common.Notifications.Messages;
using Extensoes;

namespace Api.Responses;

public class IssuerResponse
{
    public IssuerResponseType Type { get; private set; }
    public string DescriptionType { get; private set; }
    public string Title { get; set; }
    public IEnumerable<Notification>? Details { get; set; }

    public IssuerResponse(IssuerResponseType type)
    {
        Type = type;
        DescriptionType = type.Description();
    }
}
