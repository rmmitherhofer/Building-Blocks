using Microsoft.EntityFrameworkCore;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;

public interface ISecurityKeyContext
{
    DbSet<KeyMaterial> SecurityKeys { get; set; }
}
