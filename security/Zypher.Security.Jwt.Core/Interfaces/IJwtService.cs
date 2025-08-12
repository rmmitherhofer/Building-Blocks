using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Interfaces;

public interface IJwtService
{
    Task<SecurityKey> GenerateKey();
    Task<SecurityKey> GetCurrentSecurityKey();
    Task<SigningCredentials> GetCurrentSigningCredentials();
    Task<EncryptingCredentials> GetCurrentEncryptingCredentials();
    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null);
    Task RevokeKey(string keyId, string? reason = null);
    Task<SecurityKey> GenerateNewKey();
}