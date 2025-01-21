using SnapTraceV2.Models.Logger;

namespace SnapTraceV2.Factories
{
    internal class ReadStreamToTemporaryFile : IReadStreamStrategy
    {
        private readonly Stream _stream;
        public ReadStreamToTemporaryFile(Stream stream) => _stream = stream ?? throw new ArgumentNullException(nameof(stream));

        public ReadStreamResult Read()
        {
            if (!_stream.CanRead)                return new ();

            TemporaryFile file = new ();

            try
            {
                _stream.Position = 0;
                using (var fs = File.OpenWrite(file.FileName))
                {
                    _stream.CopyTo(fs);
                }
            }
            catch
            {
                file.Dispose();
                throw;
            }

            return new ReadStreamResult
            {
                TemporaryFile = file
            };
        }
    }
}
