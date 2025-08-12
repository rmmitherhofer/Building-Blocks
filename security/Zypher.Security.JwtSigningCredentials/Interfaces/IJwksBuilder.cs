using Microsoft.Extensions.DependencyInjection;

namespace Zypher.Security.JwtSigningCredentials.Interfaces;

public interface IJwksBuilder
{
    IServiceCollection Services { get; }
}