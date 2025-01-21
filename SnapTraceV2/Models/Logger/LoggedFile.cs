namespace SnapTraceV2.Models.Logger;

public class LoggedFile
{
    public string FileName { get; }
    public string FilePath { get; }
    public long FileSize { get; }

    internal LoggedFile(string fileName, string filePath, long fileSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        if (fileSize < 0) throw new ArgumentException(nameof(fileSize));

        FileName = fileName;
        FilePath = filePath;
        FileSize = fileSize;
    }

    internal LoggedFile Clone() => new(FileName, FilePath, FileSize);
}

