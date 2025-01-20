namespace SnapTrace.AspNetCore.Interfaces.Builders
{
    public interface IOptionsBuilder
    {
        LogListenersContainer Listeners { get; }
        Options Options { get; }
        Action<string> InternalLog { get; set; }
    }
}
