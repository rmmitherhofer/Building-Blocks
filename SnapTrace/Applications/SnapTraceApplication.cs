using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapTrace.Builders;
using SnapTrace.Configurations.Settings;
using SnapTrace.Extensions;
using SnapTrace.HttpServices;

namespace SnapTrace.Applications;

public class SnapTraceApplication : ISnapTraceApplication
{
    private readonly ILogContextBuilder _builder;
    private readonly ISnapTraceHttpService _httpService;
    
    private readonly SnapTraceSettings _settings;

    public SnapTraceApplication(ISnapTraceHttpService httpService, 
        IOptions<SnapTraceSettings> options, 
        ILogContextBuilder builder,
        SensitiveDataMasker sensitiveDataMasker)
    {
        _httpService = httpService;
        _settings = options.Value;
        _builder = builder;
    }

    public async Task Notify(HttpContext context, long elapsedMilliseconds)
    {
        if (!_settings.TurnOnLog) return;

        var log = await _builder
            .WithHttpContext(context)
            .WithElapsedMilliseconds(elapsedMilliseconds)
            .WithNotifications()
            .WithException()
            .BuildAsync();

        Task.Run(() => _httpService.Add(log));
    }    
}
