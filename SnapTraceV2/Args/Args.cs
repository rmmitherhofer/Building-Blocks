namespace SnapTraceV2.Args;

public class Args
{
    private readonly object[] _args;
    public Args(params object[] args) => _args = args;

    internal object[] GetArgs() => _args;
}
