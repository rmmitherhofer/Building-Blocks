using SnapTraceV2.Args;
using SnapTraceV2.Enums;
using SnapTraceV2.Factories;
using SnapTraceV2.Helpers;
using SnapTraceV2.HttpServices;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.NotifyListeners;
using SnapTraceV2.Options;
using SnapTraceV2.Services;

namespace SnapTraceV2.Listeners;

public class RequestLogsApiListener : ILogListener
{
    internal static UserOptions Options { get; } = new();

    private readonly IPublicApi _publicApi;
    private readonly Application _application;

    public ILogListenerInterceptor Interceptor { get; set; }

    public bool UseAsync { get; set; } = true;
    public string ApiUrl { get; set; }
    public bool IgnoreSslCertificate { get; set; }
    public Action<ExceptionArgs> OnException { get; set; }
    public IObfuscationService ObfuscationService { get; set; } = new ObfuscationService();

    private static readonly string[] sourceArray = ["http", "https"];

    public RequestLogsApiListener(Application application)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(Application));

        _application = application;
    }

    internal RequestLogsApiListener(Application application, IPublicApi publicApi) : this(application) => _publicApi = publicApi;

    public void OnBeginRequest(HttpRequest httpRequest) { }

    public void OnMessage(LogMessage message) { }

    public void OnFlush(FlushLogArgs args)
    {
        bool isValid = ValidateProperties();

        if (!isValid) return;

        InternalLogHelper.Log("RequestLogsApiListener: OnFlush begin", LogLevel.Trace);

        ObfuscateFlushLogArgsService obfuscateService = new(ObfuscationService);
        obfuscateService.Obfuscate(args);

        var request = PayloadFactory.Create(args);

        request.OrganizationId = _application.OrganizationId;
        request.ApplicationId = _application.ApplicationId;
        request.Keywords = InternalHelper.WrapInTryCatch(() => Options.Handlers.GenerateSearchKeywords(args));

        FlushOptions flushOptions = new()
        {
            UseAsync = UseAsync,
            OnException = OnException
        };

        IPublicApi publicApi = _publicApi ?? new PublicRestApi(ApiUrl, IgnoreSslCertificate);

        Flusher.FlushAsync(flushOptions, publicApi, args, request).ConfigureAwait(false);

        InternalLogHelper.Log("RequestLogsApiListener: OnFlush complete", LogLevel.Trace);
    }

    internal bool ValidateProperties()
    {
        string organizationId = _application.OrganizationId;
        string applicationId = _application.ApplicationId;
        string apiUrl = ApiUrl;

        if (string.IsNullOrWhiteSpace(organizationId))
        {
            InternalLogHelper.Log("RequestLogsApiListener: Application.OrganizationId is null", LogLevel.Error);
            return false;
        }

        if (string.IsNullOrWhiteSpace(applicationId))
        {
            InternalLogHelper.Log("RequestLogsApiListener: Application.applicationId is null", LogLevel.Error);
            return false;
        }

        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out Uri? uri))
        {
            InternalLogHelper.Log("RequestLogsApiListener: ApiUrl is null", LogLevel.Error);
            return false;
        }

        if (!sourceArray.Any(p => string.Compare(p, uri.Scheme, true) == 0))
        {
            InternalLogHelper.Log($"RequestLogsApiListener: ApiUrl is not a valid Uri: {uri}", LogLevel.Error);
            return false;
        }

        return true;
    }
}
