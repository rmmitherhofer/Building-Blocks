﻿namespace SnapTrace
{
    public interface ILoggerFactory
    {
        Logger Get(string categoryName = null, string url = null);
        IEnumerable<Logger> GetAll();
    }
}
