using System;

namespace SnapTrace.CloudListeners.RequestLogsListener
{
    internal class FlushOptions
    {
        public bool UseAsync { get; set; }
        public Action<ExceptionArgs> OnException { get; set; }
    }
}
