using System.Text.RegularExpressions;

namespace SnapTraceV2.Services;

public class ObfuscationService : IObfuscationService
{
    public static readonly List<string> DefaultPatterns = ["(?si)pass"];

    private readonly List<string> _patterns;
    public ObfuscationService() : this(DefaultPatterns) { }

    public ObfuscationService(IEnumerable<string> patterns)
    {
        ArgumentNullException.ThrowIfNull(patterns, nameof(IEnumerable<string>));

        _patterns = patterns.ToList();
    }

    public bool ShouldObfuscate(string key, string value, string propertyName)
    {
        //REALIZAR A OCULTAÇÃO DE DADOS SENSIVEIS

        if (string.IsNullOrWhiteSpace(key)) return false;

        foreach (string pattern in _patterns)
        {
            if (string.IsNullOrWhiteSpace(pattern)) continue;

            if (Regex.IsMatch(key, pattern, RegexOptions.Compiled)) return true;
        }

        return false;
    }

    internal List<string> GetPatterns() => _patterns;
}
