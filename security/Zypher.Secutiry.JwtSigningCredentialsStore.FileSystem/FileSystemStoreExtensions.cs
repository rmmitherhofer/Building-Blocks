using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.Secutiry.JwtSigningCredentials.Store.FileSystem;

public static class FileSystemStoreExtensions
{
    public static IJwksBuilder PersistKeysToFileSystem(this IJwksBuilder builder, DirectoryInfo directory)
    {
        builder.Services.AddScoped<IJsonWebKeyStore, FileSystemStore>(provider => new FileSystemStore(directory, provider.GetRequiredService<IOptions<JwtOptions>>(), provider.GetRequiredService<IMemoryCache>()));

        return builder;
    }
}