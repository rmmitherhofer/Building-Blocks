using System.ComponentModel;

namespace Api.Responses;

public enum IssuerResponseType
{
    [Description("NotFound")]
    NotFound,
    [Description("Validation")]
    Validation,
    [Description("Error")]
    Error
}