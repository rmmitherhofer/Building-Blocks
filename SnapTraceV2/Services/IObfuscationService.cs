namespace SnapTraceV2.Services;

public interface IObfuscationService
{
    bool ShouldObfuscate(string key, string value, string propertyName);
}
