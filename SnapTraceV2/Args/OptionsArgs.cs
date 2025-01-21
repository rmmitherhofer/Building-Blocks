using SnapTraceV2.Models.Http;
using SnapTraceV2.NotifyListeners;

namespace SnapTraceV2.Args;

public class OptionsArgs
{
    public ILogListener Listener { get; }
    public HttpProperties HttpProperties { get; }

    public OptionsArgs(ILogListener listener, HttpProperties httpProperties)
    {
        ArgumentNullException.ThrowIfNull(listener, nameof(ILogListener));

        ArgumentNullException.ThrowIfNull(httpProperties, nameof(Models.Http.HttpProperties));

        ArgumentNullException.ThrowIfNull(httpProperties, nameof(httpProperties.Response));

        Listener = listener;
        HttpProperties = httpProperties;
    }

    public class LogListenerHeaderArgs : OptionsArgs
    {
        public string HeaderName { get; }
        public string? HeaderValue { get; }

        public LogListenerHeaderArgs(ILogListener listener, HttpProperties httpProperties, string headerName, string? headerValue) : base(listener, httpProperties)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(headerName, nameof(headerName));

            HeaderName = headerName;
            HeaderValue = headerValue;
        }
    }

    public class LogListenerCookieArgs : OptionsArgs
    {
        public string CookieName { get; }
        public string? CookieValue { get; }

        public LogListenerCookieArgs(ILogListener listener, HttpProperties httpProperties, string cookieName, string? cookieValue) : base(listener, httpProperties)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cookieName, nameof(cookieName));

            CookieName = cookieName;
            CookieValue = cookieValue;
        }
    }

    public class LogListenerFormDataArgs : OptionsArgs
    {
        public string FormDataName { get; }
        public string? FormDataValue { get; }

        public LogListenerFormDataArgs(ILogListener listener, HttpProperties httpProperties, string formDataName, string? formDataValue) : base(listener, httpProperties)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(formDataName, nameof(formDataName));

            FormDataName = formDataName;
            FormDataValue = formDataValue;
        }
    }

    public class LogListenerServerVariableArgs : OptionsArgs
    {
        public string ServerVariableName { get; }
        public string? ServerVariableValue { get; }

        public LogListenerServerVariableArgs(ILogListener listener, HttpProperties httpProperties, string serverVariableName, string? serverVariableValue) : base(listener, httpProperties)
        {

            ArgumentException.ThrowIfNullOrWhiteSpace(serverVariableName, nameof(serverVariableName));

            ServerVariableName = serverVariableName;
            ServerVariableValue = serverVariableValue;
        }
    }

    public class LogListenerClaimArgs : OptionsArgs
    {
        public string ClaimType { get; }
        public string? ClaimValue { get; }

        public LogListenerClaimArgs(ILogListener listener, HttpProperties httpProperties, string claimType, string? claimValue) : base(listener, httpProperties)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(claimType, nameof(claimType));

            ClaimType = claimType;
            ClaimValue = claimValue;
        }
    }

    public class LogListenerInputStreamArgs(ILogListener listener, HttpProperties httpProperties) : OptionsArgs(listener, httpProperties) { }
}

