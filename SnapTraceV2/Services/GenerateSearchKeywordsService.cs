using SnapTraceV2.Args;
using SnapTraceV2.Models;
using System.Text.RegularExpressions;

namespace SnapTraceV2.Services;

public partial class GenerateSearchKeywordsService
{
    private static readonly Regex KeyRegex = GetKeyRegex();
    private static readonly Regex ValueRegex = GetValueRegex();

    private readonly Options _options;
    public GenerateSearchKeywordsService() : this(new Options()) { }
    public GenerateSearchKeywordsService(Options options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(Options));
        _options = options;
    }

    public IEnumerable<string> GenerateKeywords(FlushLogArgs flushLogArgs)
    {
        ArgumentNullException.ThrowIfNull(flushLogArgs, nameof(FlushLogArgs));

        return (List<string>)([
            .. Get(flushLogArgs.HttpProperties.Request.Properties.QueryString),
            .. Get(flushLogArgs.HttpProperties.Request.Properties.FormData),
            .. GetFromInputStream(flushLogArgs.HttpProperties.Request.Properties.InputStream),
            .. Get(flushLogArgs.Exceptions),
        ]);
    }

    private IEnumerable<string> Get(IEnumerable<KeyValuePair<string, string?>> values)
    {
        List<string> result = [];

        foreach (var item in values)
        {
            if (string.IsNullOrWhiteSpace(item.Key) || item.Key.Length < _options.KeyCriteria.MinimumLength || item.Key.Length > _options.KeyCriteria.MaximumLength)
                continue;

            if (string.IsNullOrWhiteSpace(item.Value) || item.Value.Length < _options.ValueCriteria.MinimumLength || item.Value.Length > _options.ValueCriteria.MaximumLength)
                continue;

            if (_options.KeyCriteria.Pattern?.IsMatch(item.Key) == false)
                continue;

            if (_options.ValueCriteria.Pattern?.IsMatch(item.Value) == false)
                continue;

            string value = $"{item.Key.Trim()}:{item.Value.Trim()}";
            result.Add(value);
        }

        return result;
    }

    private IEnumerable<string> Get(IEnumerable<CapturedException> values)
        => (List<string>)([.. values.Select(p => p.Type), .. values.Select(p => p.Message)]);

    private IEnumerable<string> GetFromInputStream(string inputStream)
    {
        List<KeyValuePair<string, string?>> values = [];

        if (SnapTraceOptionsConfiguration.JsonSerializer.IsJson(inputStream))
        {
            var properties = SnapTraceOptionsConfiguration.JsonSerializer.DeserializeAndFlatten(inputStream);

            values = properties.Where(p => p.Value is not null && IsSimpleType(p.Value.GetType()))
                .Select(p => new KeyValuePair<string, string?>(p.Key, p.Value.ToString())).ToList();
        }
        return Get(values);
    }

    private bool IsSimpleType(Type type) 
        => type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(Guid);

    public class Options
    {
        public const int MIN_LENGTH = 1;
        public const int MAX_LENGTH = 100;

        public Criteria KeyCriteria { get; }
        public Criteria ValueCriteria { get; }

        public Options() : this(new Criteria(MIN_LENGTH, MAX_LENGTH, KeyRegex), new Criteria(MIN_LENGTH, MAX_LENGTH, ValueRegex)) { }

        public Options(Criteria keyCriteria, Criteria valueCriteria)
        {
            ArgumentNullException.ThrowIfNull(keyCriteria, nameof(Criteria));
            ArgumentNullException.ThrowIfNull(valueCriteria, nameof(Criteria));

            KeyCriteria = keyCriteria;
            ValueCriteria = valueCriteria;
        }
    }

    public class Criteria
    {
        public int MinimumLength { get; }
        public int MaximumLength { get; }
        public Regex? Pattern { get; }

        public Criteria(int minimumLength, int maximumLength) : this(minimumLength, maximumLength, null) { }

        public Criteria(int minimumLength, int maximumLength, Regex? pattern)
        {
            if (minimumLength < 1)
                throw new ArgumentException($"{nameof(minimumLength)} must be greater or equal to 1", nameof(minimumLength));

            if (maximumLength < minimumLength)
                throw new ArgumentException($"{nameof(minimumLength)} must be greater or equal to {nameof(minimumLength)}", nameof(maximumLength));

            MinimumLength = minimumLength;
            MaximumLength = maximumLength;
            Pattern = pattern;
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_-]*$", RegexOptions.Compiled)]
    private static partial Regex GetKeyRegex();
    [GeneratedRegex(@"^[a-zA-Z0-9_\-\+\/.@: ]*$", RegexOptions.Compiled)]
    private static partial Regex GetValueRegex();
}
