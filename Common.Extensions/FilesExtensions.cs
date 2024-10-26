using Newtonsoft.Json;
using System.Text;

namespace Extensoes;

public static class FilesExtensions
{
    public static T JsonToObject<T>(string path) 
        => JsonConvert.DeserializeObject<T>(File.ReadAllText(path, Encoding.GetEncoding("UTF-8")));
}
