namespace SnapTraceV2.Exceptions;

internal class LogByteArrayAsFileException : Exception
{
    public LogByteArrayAsFileException(byte[] contents, Exception innerException) : base(ErrorMessage(contents), innerException) { }
    private static string ErrorMessage(byte[] contents)
    {
        int length = (contents?.Length) ?? 0;
        return $"{Constants.PackageName}: Error when trying to log as file the byte[] with length {length}";
    }
}
