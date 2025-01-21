using SnapTraceV2.Models;
using System.Text.RegularExpressions;

namespace SnapTraceV2;

public partial class Constants
{
    public static readonly string PackageName = "SnapTrace";
    public static readonly Regex FileNameRegex = GetFileNameRegex();
    public static readonly Regex UrlRegex = GetUrlRegex();
    public static readonly Regex HttpMethodRegex = GetHttpMethodRegex();

    public static readonly string[] ReadInputStreamContentTypes = ["application/javascript", "application/json", "application/xml", "text/plain", "text/xml", "text/html"];
    public static readonly string[] ReadResponseBodyContentTypes = ["application/json", "application/xml", "text/html", "text/plain", "text/xml"];
    public static readonly string[] DefaultReadResponseBodyContentTypes = ["application/json"];

    public const string DefaultLoggerCategory = "Default";
    public const long MaximumAllowedFileSizeInBytes = 5 * 1024 * 1024;
    public const long ReadStreamAsStringMaxContentLengthInBytes = 1 * 1024 * 1024;
    public const string DefaultBaseUrl = "http://application";

    internal static SnapTracePackage UnknownPackage => new(PackageName + ".Unknown", new Version(1, 0, 0));

    [GeneratedRegex(@"[^a-zA-Z0-9_\-\+\. ]+", RegexOptions.Compiled)]
    private static partial Regex GetFileNameRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9/:._\+\=\&\-\?]+", RegexOptions.Compiled)]
    private static partial Regex GetUrlRegex();
    [GeneratedRegex(@"[^a-zA-Z0-9]+", RegexOptions.Compiled)]
    private static partial Regex GetHttpMethodRegex();
}
