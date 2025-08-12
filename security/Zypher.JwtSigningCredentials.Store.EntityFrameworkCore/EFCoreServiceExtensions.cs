using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;

public static class EFCoreServiceExtensions
{
    public static IJwksBuilder PersistKeysToDatabaseStore<TDbContext>(this IJwksBuilder builder) where TDbContext : DbContext, ISecurityKeyContext
    {
        builder.Services.AddScoped<IJsonWebKeyStore, DatabaseJsonWebKeyStore<TDbContext>>();

        return builder;
    }
}