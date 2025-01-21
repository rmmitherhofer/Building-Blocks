using SnapTraceV2.Args;
using SnapTraceV2.Decorators;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.Services;

namespace SnapTraceV2.NotifyListeners;

internal static class NotifyFlush
{
    public static void Notify(LoggerService[] loggers)
    {
        ArgumentNullException.ThrowIfNull(loggers, nameof(LoggerService));

        if (loggers.Length == 0) return;

        var args = FlushLogArgsFactory.Create(loggers);

        Guid? httpRequestId = args.HttpProperties?.Request.Id;

        foreach (LogListenerDecorator decorator in SnapTraceOptionsConfiguration.Listeners.GetAll())
        {
            InternalHelper.WrapInTryCatch(() => Notify(args, decorator, httpRequestId));
        }

        foreach (LoggerService logger in loggers)
        {
            logger.Reset();
        }
    }

    private static void Notify(FlushLogArgs args, LogListenerDecorator decorator, Guid? httpRequestId)
    {
        if (httpRequestId != null && decorator.SkipHttpRequestIds.Contains(httpRequestId.Value)) return;

        ILogListener listener = decorator.Listener;

        if (listener.Interceptor?.ShouldLog(args, listener) == false) return;

        FlushLogArgs argsForListener = CreateArgsForListener(args, listener);

        listener.OnFlush(argsForListener);
    }

    internal static FlushLogArgs CreateArgsForListener(FlushLogArgs flushLogArgs, ILogListener listener)
    {
        //PREPARA OS DADOS PARA CRIAR O MODELO DO REQUEST

        ArgumentNullException.ThrowIfNull(flushLogArgs, nameof(FlushLogArgs));

        ArgumentNullException.ThrowIfNull(listener, nameof(ILogListener));

        List<KeyValuePair<string, string?>> requestHeaders = [];
        List<KeyValuePair<string, string?>> requestCookies = [];
        List<KeyValuePair<string, string?>> requestFormData = [];
        List<KeyValuePair<string, string?>> requestServerVariables = [];
        List<KeyValuePair<string, string>> requestClaims = [];
        List<KeyValuePair<string, string?>> responseHeaders = [];
        string? inputStream = null;

        foreach (var item in flushLogArgs.HttpProperties.Request.Properties.Headers)
        {
            var args = new OptionsArgs.LogListenerHeaderArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);

            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogRequestHeaderForListener.Invoke(args);

            if (shouldLog) requestHeaders.Add(item);
        }

        foreach (var item in flushLogArgs.HttpProperties.Request.Properties.Cookies)
        {
            var args = new OptionsArgs.LogListenerCookieArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);
            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogRequestCookieForListener.Invoke(args);

            if (shouldLog) requestCookies.Add(item);
        }

        foreach (var item in flushLogArgs.HttpProperties.Request.Properties.FormData)
        {
            var args = new OptionsArgs.LogListenerFormDataArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);
            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogFormDataForListener.Invoke(args);

            if (shouldLog) requestFormData.Add(item);
        }

        foreach (var item in flushLogArgs.HttpProperties.Request.Properties.ServerVariables)
        {
            var args = new OptionsArgs.LogListenerServerVariableArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);
            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogServerVariableForListener.Invoke(args);

            if (shouldLog) requestServerVariables.Add(item);
        }

        foreach (var item in flushLogArgs.HttpProperties.Request.Properties.Claims)
        {
            var args = new OptionsArgs.LogListenerClaimArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);
            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogClaimForListener.Invoke(args);

            if (shouldLog) requestClaims.Add(item);
        }

        if (SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogInputStreamForListener(new OptionsArgs.LogListenerInputStreamArgs(listener, flushLogArgs.HttpProperties)))
            inputStream = flushLogArgs.HttpProperties.Request.Properties.InputStream;

        foreach (var item in flushLogArgs.HttpProperties.Response.Properties.Headers)
        {
            var args = new OptionsArgs.LogListenerHeaderArgs(listener, flushLogArgs.HttpProperties, item.Key, item.Value);
            bool shouldLog = SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogResponseHeaderForListener.Invoke(args);

            if (shouldLog) responseHeaders.Add(item);
        }

        List<LogMessagesGroup> messagesGroups = [];
        foreach (var group in flushLogArgs.MessagesGroups)
        {
            List<LogMessage> messages = group.Messages.ToList();

            if (listener.Interceptor is not null)
            {
                messages = messages.Where(p => listener.Interceptor.ShouldLog(p, listener)).ToList();
            }

            messagesGroups.Add(new LogMessagesGroup(group.Category, messages));
        }

        FlushLogArgs result = flushLogArgs.Clone();

        result.SetMessagesGroups(messagesGroups);

        result.HttpProperties.Request.SetProperties(new RequestProperties(new RequestProperties.CreateOptions
        {
            Claims = requestClaims,
            Cookies = requestCookies,
            FormData = requestFormData,
            Headers = requestHeaders,
            QueryString = flushLogArgs.HttpProperties.Request.Properties.QueryString.ToList(),
            ServerVariables = requestServerVariables,
            InputStream = inputStream
        }));

        result.HttpProperties.Response.SetProperties(new ResponseProperties(new ResponseProperties.CreateOptions
        {
            Headers = responseHeaders,
            ContentLength = flushLogArgs.HttpProperties.Response.Properties.ContentLength
        }));

        return result;
    }
}
