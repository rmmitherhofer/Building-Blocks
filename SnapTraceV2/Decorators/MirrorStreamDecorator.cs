using SnapTraceV2.NotifyListeners;
using System.Text;

namespace SnapTraceV2.Decorators;

internal class MirrorStreamDecorator : Stream
{
    private readonly Stream _decorated;
    public MemoryStream MirrorStream { get; }
    public Encoding Encoding { get; }

    public MirrorStreamDecorator(Stream decorated, Encoding? encoding = null)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        Encoding = encoding ?? Encoding.UTF8;
        MirrorStream = new();
    }

    protected MirrorStreamDecorator(MemoryStream mirrorStream) => MirrorStream = mirrorStream;

    public override bool CanRead => _decorated.CanRead;
    public override bool CanSeek => _decorated.CanSeek;
    public override bool CanWrite => _decorated.CanWrite;
    public override long Length => _decorated.Length;
    public override long Position { get => _decorated.Position; set => _decorated.Position = value; }

    public override int Read(byte[] buffer, int offset, int count) =>
        _decorated.Read(buffer, offset, count);
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => _decorated.ReadAsync(buffer, offset, count, cancellationToken);

    public override void Write(byte[] buffer, int offset, int count)
    {
        _decorated.Write(buffer, offset, count);

        if (MirrorStream?.CanWrite == true)
            MirrorStream.Write(buffer, offset, count);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        await _decorated.WriteAsync(buffer, offset, count, cancellationToken);

        if (MirrorStream?.CanWrite == true)
            MirrorStream.Write(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin) => _decorated.Seek(offset, origin);

    public override void SetLength(long value)
    {
        _decorated.SetLength(value);

        if (MirrorStream?.CanWrite == true)
            MirrorStream.SetLength(value);
    }

    public override void Flush() => _decorated.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => _decorated.FlushAsync(cancellationToken);

    public override void Close() => _decorated.Close();


}

internal class LogListenerDecorator
{
    public ILogListener Listener { get; }
    public List<Guid> SkipHttpRequestIds { get; }

    public LogListenerDecorator(ILogListener listener)
    {
        Listener = listener ?? throw new ArgumentNullException(nameof(listener));
        SkipHttpRequestIds = [];
    }
}
