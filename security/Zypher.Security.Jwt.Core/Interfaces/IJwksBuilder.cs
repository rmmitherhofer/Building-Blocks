using Microsoft.Extensions.DependencyInjection;

namespace Zypher.Security.Jwt.Core.Interfaces;

public interface IJwksBuilder
{
    IServiceCollection Services { get; }
}
