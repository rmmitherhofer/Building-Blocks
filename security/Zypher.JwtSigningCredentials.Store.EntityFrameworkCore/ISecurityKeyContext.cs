using Microsoft.EntityFrameworkCore;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.Store.EntityFrameworkCore;

public interface ISecurityKeyContext
{
    DbSet<KeyMaterial> SecurityKeys { get; set; }
}
