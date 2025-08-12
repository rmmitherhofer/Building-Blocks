using Microsoft.Extensions.DependencyInjection;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.Security.Jwt.Core;

public class JwksBuilder(IServiceCollection services) : IJwksBuilder
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services), $"{nameof(IServiceCollection)} is not null");
}
