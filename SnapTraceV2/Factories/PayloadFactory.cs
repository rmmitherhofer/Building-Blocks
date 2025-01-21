using SnapTraceV2.Args;
using SnapTraceV2.Listeners;
using SnapTraceV2.Models;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.Models.Requests;

namespace SnapTraceV2.Factories;

internal static class PayloadFactory
{
    public static CreateRequestLogRequest Create(FlushLogArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(FlushLogArgs));

        var package = SnapTraceOptionsConfiguration.SnapTracePackages.GetPrimaryPackage();

        DateTime startDateTime = args.HttpProperties.Request.StartDateTime;
        DateTime endDateTime = args.HttpProperties.Response.EndDateTime;

        IEnumerable<LogMessage> logMessages = args.MessagesGroups.SelectMany(p => p.Messages).OrderBy(p => p.DateTime).ToList();

        return new()
        {
            SdkName = package.Name,
            SdkVersion = package.Version.ToString(),
            StartDateTime = startDateTime,
            DurationInMilliseconds = Math.Max(0, (endDateTime - startDateTime).TotalMilliseconds),
            WebRequest = Create(args.HttpProperties),
            MachineName = args.HttpProperties.Request.MachineName,
            IsNewSession = args.HttpProperties.Request.IsNewSession,
            SessionId = args.HttpProperties.Request.SessionId,
            IsAuthenticated = args.HttpProperties.Request.IsAuthenticated,
            User = CreateUser(args.HttpProperties.Request),
            LogMessages = logMessages.Select(p => Create(p, startDateTime)).ToList(),
            Exceptions = args.Exceptions?.Select(p => Create(p)).ToList(),
            CustomProperties = args.CustomProperties.ToList()
        };
    }

    internal static HttpPropertiesRequest Create(HttpProperties httpProperties)
    {
        ArgumentNullException.ThrowIfNull(httpProperties, nameof(HttpProperties));

        var url = Create(httpProperties.Request.Url);
        var requestProperties = Create(httpProperties.Request.Properties);
        var responseProperties = Create(httpProperties.Response);

        return new()
        {
            Url = url,
            UserAgent = httpProperties.Request.UserAgent,
            HttpMethod = httpProperties.Request.HttpMethod,
            HttpReferer = httpProperties.Request.HttpReferer,
            RemoteAddress = httpProperties.Request.RemoteAddress,
            Request = requestProperties,
            Response = responseProperties
        };
    }

    internal static RequestPropertiesRequest Create(RequestProperties requestProperties)
    {
        ArgumentNullException.ThrowIfNull(requestProperties, nameof(RequestProperties));

        return new()
        {
            Cookies = requestProperties.Cookies.ToList(),
            Headers = requestProperties.Headers.ToList(),
            Claims = requestProperties.Claims.ToList(),
            QueryString = requestProperties.QueryString.ToList(),
            FormData = requestProperties.FormData.ToList(),
            ServerVariables = requestProperties.ServerVariables.ToList(),
            InputStream = requestProperties.InputStream
        };
    }

    internal static ResponsePropertiesRequest Create(HttpResponse httpResponse)
    {
        ArgumentNullException.ThrowIfNull(httpResponse, nameof(HttpResponse));

        return new()
        {
            HttpStatusCode = httpResponse.StatusCode,
            HttpStatusCodeText = ((System.Net.HttpStatusCode)httpResponse.StatusCode).ToString(),
            Headers = httpResponse.Properties.Headers.ToList(),
            ContentLength = httpResponse.Properties.ContentLength,
        };
    }

    internal static LogMessageRequest Create(LogMessage message, DateTime startRequestDateTime)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(LogMessage));

        return new()
        {
            CategoryName = message.Category,
            LogLevel = message.LogLevel.ToString(),
            Message = message.Message,
            MillisecondsSinceStartRequest = Math.Max(0, (message.DateTime - startRequestDateTime).TotalMilliseconds),
            MemberType = message.MemberType,
            MemberName = message.MemberName,
            LineNumber = message.LineNumber
        };
    }

    internal static UrlRequest Create(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(Uri));

        return new()
        {
            Path = uri.LocalPath,
            PathAndQuery = uri.PathAndQuery,
            AbsoluteUri = uri.AbsoluteUri
        };
    }

    internal static CapturedExceptionRequest Create(CapturedException exception)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(CapturedException));

        return new()
        {
            ExceptionType = exception.Type,
            ExceptionMessage = exception.Message
        };
    }

    internal static UserRequest? CreateUser(HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest, nameof(HttpRequest));

        var user = RequestLogsApiListener.Options.Handlers.CreateUserPayload.Invoke(httpRequest);

        return IsNull(user) ? null : user;
    }

    private static bool IsNull(UserRequest user)
    {
        if (user is null) return true;

        if (string.IsNullOrWhiteSpace(user.Name) && string.IsNullOrWhiteSpace(user.EmailAddress))
            return true;

        return false;
    }
}
