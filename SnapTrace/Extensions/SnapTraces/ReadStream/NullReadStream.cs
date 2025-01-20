namespace SnapTrace.ReadStream
{
    internal class NullReadStream : IReadStreamStrategy
    {
        public ReadStreamResult Read()
        {
            return new ReadStreamResult();
        }
    }
}
