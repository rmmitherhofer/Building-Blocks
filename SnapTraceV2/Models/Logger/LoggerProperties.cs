﻿namespace SnapTraceV2.Models.Logger;

internal class LoggerProperties
{
    public int? ExplicitStatusCode { get; set; }
    public bool? ExplicitLogResponseBody { get; set; }
    public bool IsManagedByHttpRequest { get; set; }
    public List<KeyValuePair<string, object>> CustomProperties { get; }

    public LoggerProperties() => CustomProperties = [];
}
