using SnapTrace.RestClient;
using SnapTrace.RestClient.Api;
using SnapTrace.RestClient.Models;
using SnapTrace.RestClient.Requests.CreateRequestLog;

namespace SnapTrace.CloudListeners.RequestLogsListener
{
    internal static class Flusher
    {
        public static async Task FlushAsync(FlushOptions options, IPublicApi kisslogApi, FlushLogArgs flushArgs, CreateRequestLogRequest request)
        {
            IEnumerable<LoggedFile> files = CopyFiles(flushArgs);
            flushArgs.SetFiles(files);

            IEnumerable<RestClient.Requests.CreateRequestLog.File> requestFiles = files.Select(p => new RestClient.Requests.CreateRequestLog.File
            {
                FileName = p.FileName,
                FilePath = p.FilePath
            }).ToList();

            try
            {
                ApiResult<RequestLog> result = null;

                if (options.UseAsync)
                {
                    result = await kisslogApi.CreateRequestLogAsync(request, requestFiles).ConfigureAwait(false);
                }
                else
                {
                    result = kisslogApi.CreateRequestLog(request, requestFiles);
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
            if (args.Files == null || !args.Files.Any())
                return new List<LoggedFile>();

            List<LoggedFile> result = new List<LoggedFile>();

            foreach (var file in args.Files)
            {
                if (!System.IO.File.Exists(file.FilePath))
                    continue;

                TemporaryFile tempFile = new TemporaryFile();
                System.IO.File.Copy(file.FilePath, tempFile.FileName, true);

                result.Add(new LoggedFile(file.FileName, tempFile.FileName, file.FileSize));
            }

            return result;
        }

        private static void DeleteFiles(IEnumerable<LoggedFile> files)
        {
            if (files == null || !files.Any())
                return;

            foreach (var item in files)
            {
                if (System.IO.File.Exists(item.FilePath))
                {
                    try
                    {
                        System.IO.File.Delete(item.FilePath);
                    }
                    catch (Exception ex)
                    {
                        InternalLogger.LogException(ex);
                    }
                }
            }
        }
    }
}
