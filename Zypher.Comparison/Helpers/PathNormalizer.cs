namespace Zypher.Comparison.Helpers;

public static class PathNormalizer
{
    public static string NormalizePath(string path)
    {
        return NormalizePath(path, removeIndexes: false);
    }

    public static string NormalizePath(string path, bool removeIndexes)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        const string rootPrefix = "$.";
        var start = path.StartsWith(rootPrefix, StringComparison.Ordinal) ? rootPrefix.Length : 0;
        if (!removeIndexes)
        {
            return start == 0 ? path : path[start..];
        }

        var sourceLength = path.Length - start;
        var buffer = new char[sourceLength];
        var index = 0;
        var depth = 0;

        for (var i = start; i < path.Length; i++)
        {
            var character = path[i];
            if (character == '[')
            {
                depth++;
                continue;
            }

            if (character == ']')
            {
                if (depth > 0)
                {
                    depth--;
                }

                continue;
            }

            if (depth == 0)
            {
                buffer[index++] = character;
            }
        }

        return index == sourceLength ? path[start..] : new string(buffer, 0, index);
    }

    public static string RemoveArrayIndexes(string path)
    {
        return NormalizePath(path, removeIndexes: true);
    }
}
