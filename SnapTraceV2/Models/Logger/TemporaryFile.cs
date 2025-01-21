namespace SnapTraceV2.Models.Logger;

internal class TemporaryFile : IDisposable
{
    internal static readonly string[] AllowedExtensions = ["tmp", "png", "jpg", "jpeg", "jfif", "gif", "bm", "bmp", "txt", "log", "pdf"];
    internal const string DefaultExtension = "tmp";

    public string? FileName { get; private set; }

    internal bool _disposed;

    private static string GenerateSalt() => Guid.NewGuid().ToString("N")[..12];

    private static string GetTempFileName(string? extension = null)
    {
        if (string.IsNullOrWhiteSpace(extension)) extension = DefaultExtension;

        extension = extension.Replace(".", string.Empty).Trim().ToLowerInvariant();

        if (!AllowedExtensions.Any(p => extension == p)) extension = DefaultExtension;

        string? path = Path.Combine(Path.GetTempPath(), "SnapTrace", $"{GenerateSalt()}.{extension}");

        try
        {
            FileInfo fi = new(path);
            fi?.Directory!.Create();

            using (File.Create(path)) { };
        }
        catch
        {
            path = null;
        }

        if (path is null)
        {
            try
            {
                path = Path.Combine(Path.GetTempPath(), $"SnapTrace_{GenerateSalt()}.{extension}");
                using (File.Create(path)) { };
            }
            catch
            {
                path = null;
            }
        }

        path ??= Path.GetTempFileName();

        return path;
    }

    public TemporaryFile() : this(null) { }

    public TemporaryFile(string extension) => FileName = GetTempFileName(extension);

    public long GetSize()
    {
        if (_disposed) return 0;

        if(string.IsNullOrEmpty(FileName)) return 0;

        FileInfo fileInfo = new(FileName);

        if (fileInfo.Exists) return fileInfo.Length;

        return 0;
    }

    public void Dispose()
    {
        try
        {
            if (FileName is not null && File.Exists(FileName))
                File.Delete(FileName);

            FileName = null;
        }
        catch { }

        _disposed = true;
    }
}
