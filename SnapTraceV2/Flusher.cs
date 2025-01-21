using SnapTraceV2.Args;
using SnapTraceV2.Helpers;
using SnapTraceV2.HttpServices;
using SnapTraceV2.Models;
using SnapTraceV2.Models.Logger;
using SnapTraceV2.Models.Requests;
using SnapTraceV2.Models.Responses;
using SnapTraceV2.Options;

namespace SnapTraceV2;

internal static class Flusher
{
    public static async Task FlushAsync(FlushOptions options, IPublicApi publicApi, FlushLogArgs flushArgs, CreateRequestLogRequest request)
    {
        IEnumerable<LoggedFile> files = CopyFiles(flushArgs);
        flushArgs.SetFiles(files);

        IEnumerable<FileRequest> requestFiles = files.Select(p => new FileRequest
        {
            FileName = p.FileName,
            FilePath = p.FilePath
        }).ToList();

        try
        {
            ApiResult<RequestLogResponse>? result = null;

            if (options.UseAsync)
            {
                result = await publicApi.CreateRequestLogAsync(request, requestFiles).ConfigureAwait(false);
            }
            else
            {
                result = publicApi.CreateRequestLog(request, requestFiles);
            }

            if (result.HasException && options.OnException != null)
            {
                options.OnException.Invoke(new ExceptionArgs(flushArgs, result));
            }
        }
        finally
        {
            DeleteFiles(files);
        }
    }

    private static IEnumerable<LoggedFile> CopyFiles(FlushLogArgs args)
    {
        if (args.Files?.Any() != true) return [];

        List<LoggedFile> result = [];

        foreach (var file in args.Files)
        {
            if (!File.Exists(file.FilePath))
                continue;

            TemporaryFile tempFile = new TemporaryFile();
            File.Copy(file.FilePath, tempFile.FileName, true);

            result.Add(new LoggedFile(file.FileName, tempFile.FileName, file.FileSize));
        }

        return result;
    }

    private static void DeleteFiles(IEnumerable<LoggedFile> files)
    {
        if (files?.Any() != true) return;

        foreach (var item in files)
        {
            if (File.Exists(item.FilePath))
            {
                try
                {
                    File.Delete(item.FilePath);
                }
                catch (Exception ex)
                {
                    InternalLogHelper.LogException(ex);
                }
            }
        }
    }
}
