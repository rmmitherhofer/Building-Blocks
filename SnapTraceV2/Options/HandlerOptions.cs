using SnapTraceV2.Args;
using SnapTraceV2.Models.Http;

namespace SnapTraceV2.Options;

public class HandlerOptions
{
    internal HandlersContainer Handlers { get; }
    public HandlerOptions() => Handlers = new HandlersContainer();

    public HandlerOptions AppendExceptionDetails(Func<Exception, string> handler)
    {
        if (handler is null) return this;

        Handlers.AppendExceptionDetails = handler;
        return this;
    }

    public HandlerOptions ShouldLogRequestHeader(Func<OptionsArgs.LogListenerHeaderArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogRequestHeaderForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogRequestCookie(Func<OptionsArgs.LogListenerCookieArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogRequestCookieForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogFormData(Func<OptionsArgs.LogListenerFormDataArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogFormDataForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogServerVariable(Func<OptionsArgs.LogListenerServerVariableArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogServerVariableForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogClaim(Func<OptionsArgs.LogListenerClaimArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogClaimForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogInputStream(Func<OptionsArgs.LogListenerInputStreamArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogInputStreamForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogResponseHeader(Func<OptionsArgs.LogListenerHeaderArgs, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogResponseHeaderForListener = handler;
        return this;
    }

    public HandlerOptions ShouldLogFormData(Func<HttpRequest, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogFormData = handler;
        return this;
    }

    public HandlerOptions ShouldLogInputStream(Func<HttpRequest, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogInputStream = handler;
        return this;
    }

    public HandlerOptions ShouldLogResponseBody(Func<HttpProperties, bool> handler)
    {
        if (handler is null) return this;

        Handlers.ShouldLogResponseBody = handler;
        return this;
    }

    internal class HandlersContainer
    {
        public Func<Exception, string> AppendExceptionDetails { get; set; }
        public Func<OptionsArgs.LogListenerHeaderArgs, bool> ShouldLogRequestHeaderForListener { get; set; }
        public Func<OptionsArgs.LogListenerCookieArgs, bool> ShouldLogRequestCookieForListener { get; set; }
        public Func<OptionsArgs.LogListenerFormDataArgs, bool> ShouldLogFormDataForListener { get; set; }
        public Func<OptionsArgs.LogListenerServerVariableArgs, bool> ShouldLogServerVariableForListener { get; set; }
        public Func<OptionsArgs.LogListenerClaimArgs, bool> ShouldLogClaimForListener { get; set; }
        public Func<OptionsArgs.LogListenerInputStreamArgs, bool> ShouldLogInputStreamForListener { get; set; }
        public Func<OptionsArgs.LogListenerHeaderArgs, bool> ShouldLogResponseHeaderForListener { get; set; }
        public Func<HttpRequest, bool> ShouldLogFormData { get; set; }
        public Func<HttpRequest, bool> ShouldLogInputStream { get; set; }
        public Func<HttpProperties, bool> ShouldLogResponseBody { get; set; }

        public HandlersContainer()
        {
            AppendExceptionDetails = (ex) => null;
            ShouldLogRequestHeaderForListener = (args) => true;
            ShouldLogRequestCookieForListener = (args) => true;
            ShouldLogFormDataForListener = (args) => true;
            ShouldLogServerVariableForListener = (args) => true;
            ShouldLogClaimForListener = (args) => true;
            ShouldLogInputStreamForListener = (args) => true;
            ShouldLogResponseHeaderForListener = (args) => true;
            ShouldLogFormData = (args) => true;
            ShouldLogInputStream = (args) => true;
            ShouldLogResponseBody = (args) =>
            {
                string? contentType = args.Response?.Properties?.Headers?.FirstOrDefault(p => string.Compare(p.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0).Value;

                if (string.IsNullOrEmpty(contentType)) return false;

                contentType = contentType.Trim().ToLowerInvariant();

                return Constants.DefaultReadResponseBodyContentTypes.Any(p => contentType.Contains(p));
            };
        }
    }
}
