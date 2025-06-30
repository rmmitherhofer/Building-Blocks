namespace SnapTrace.Formatters
{
    public class FormatterArgs
    {
        public object State { get; }
        public Exception Exception { get; }
        public string DefaultValue { get; }

        internal FormatterArgs(CreateOptions options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

            State = options.State;
            Exception = options.Exception;
            DefaultValue = options.DefaultValue;
        }

        internal class CreateOptions
        {
            public object State { get; set; }
            public Exception Exception { get; set; }
            public string DefaultValue { get; set; }
        }
    }
}
