using Microsoft.Extensions.DependencyInjection;
using Zypher.Security.JwtSigningCredentials.Interfaces;

namespace Zypher.Security.JwtSigningCredentials;

public class JwksBuilder(IServiceCollection services) : IJwksBuilder
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}
