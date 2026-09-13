using System.Security.Cryptography;
using System.Text;
using KernixERP.Interfaces;

namespace KernixERP.Security.JWT;

public sealed class TokenService : ITokenService
{
    public string GenerarRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string CalcularHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}