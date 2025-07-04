namespace SnapTrace.Configurations.Settings;

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
    internal class HandlersContainer
    {
        public Func<Exception, string> AppendExceptionDetails { get; set; }

        public HandlersContainer()
        {
            AppendExceptionDetails = (ex) => null;
        }
    }
}


public class SensitiveDataMaskerOptions
{
    public List<string> SensitiveKeys { get; set; } =
    [
        "password", "senha", "token", "accessToken", "signature", "assinatura", "secret", "key", "pin"
    ];
}