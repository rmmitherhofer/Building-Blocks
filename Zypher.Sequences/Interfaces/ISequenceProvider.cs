namespace Zypher.Sequences.Interfaces;

public interface ISequenceProvider
{
    Task<long> NextAsync(string scopeKey, CancellationToken cancellationToken = default);
}
