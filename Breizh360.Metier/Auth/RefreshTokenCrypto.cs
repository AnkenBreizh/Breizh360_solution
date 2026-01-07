using System.Security.Cryptography;
using System.Text;

namespace Breizh360.Metier.Auth;

public static class RefreshTokenCrypto
{
    public static string GenerateOpaqueToken(int bytes = 64)
    {
        var buffer = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(buffer);
    }

    public static string HashToken(string refreshToken, string signingKey)
    {
        // HMAC du refresh token (stockage DB de ce hash, pas du token brut)
        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }
}
