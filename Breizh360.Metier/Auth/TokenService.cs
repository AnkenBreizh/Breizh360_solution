using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Breizh360.Metier.Common;
using Microsoft.IdentityModel.Tokens;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-002 — Émission JWT + refresh token (hashé + rotation).
/// Remise attendue : Breizh360.Metier/Auth/TokenService.cs
/// </summary>
public sealed class TokenService
{
    // TODO: remplacer par les interfaces Domaine (ex: IRefreshTokenRepository, IUserRepository, etc.)
    private readonly dynamic _refreshTokenRepository;
    private readonly AuthOptionsJwt _jwt;
    private readonly IClock _clock;

    public TokenService(dynamic refreshTokenRepository, AuthOptionsJwt jwtOptions, IClock clock)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwt = jwtOptions;
        _clock = clock;
    }

    public async Task<TokenPair> IssueAsync(
        Guid userId,
        string login,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        CancellationToken ct)
    {
        var now = _clock.UtcNow;

        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var accessToken = CreateJwt(userId, login, roles, permissions, now, accessExp);

        var refreshToken = RefreshTokenCrypto.GenerateOpaqueToken();
        var refreshHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);
        var refreshExp = now.AddDays(_jwt.RefreshTokenDays);

        // TODO: persister refresh token hashé (entity Domaine RefreshToken)
        await _refreshTokenRepository.CreateAsync(new
        {
            UserId = userId,
            TokenHash = refreshHash,
            ExpiresAtUtc = refreshExp,
            CreatedAtUtc = now,
            RevokedAtUtc = (DateTimeOffset?)null,
            ReplacedByTokenHash = (string?)null
        }, ct);

        return new TokenPair(accessToken, refreshToken, accessExp, refreshExp);
    }

    public async Task<TokenPair> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var now = _clock.UtcNow;
        var refreshHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);

        // TODO: charger l’enregistrement refresh par hash
        var stored = await _refreshTokenRepository.GetByHashAsync(refreshHash, ct);
        if (stored is null)
            throw new SecurityTokenException("Refresh token invalide.");

        // TODO: vérifier expiré / révoqué
        // ex: if (stored.RevokedAtUtc != null || stored.ExpiresAtUtc <= now) throw ...

        // TODO: rotation : revoke ancien + créer nouveau
        var newRefreshToken = RefreshTokenCrypto.GenerateOpaqueToken();
        var newRefreshHash = RefreshTokenCrypto.HashToken(newRefreshToken, _jwt.SigningKey);
        var newRefreshExp = now.AddDays(_jwt.RefreshTokenDays);

        await _refreshTokenRepository.RotateAsync(
            oldTokenHash: refreshHash,
            newTokenHash: newRefreshHash,
            newExpiresAtUtc: newRefreshExp,
            rotatedAtUtc: now,
            ct: ct
        );

        // TODO: récupérer user + rôles/permissions selon Domaine, ou les stocker sur stored
        Guid userId = stored.UserId;
        string login = stored.Login; // adapter si besoin
        IEnumerable<string> roles = stored.Roles; // adapter si besoin
        IEnumerable<string> permissions = stored.Permissions; // adapter si besoin

        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var accessToken = CreateJwt(userId, login, roles, permissions, now, accessExp);

        return new TokenPair(accessToken, newRefreshToken, accessExp, newRefreshExp);
    }

    private string CreateJwt(
        Guid userId,
        string login,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        DateTimeOffset now,
        DateTimeOffset exp)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, login),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        // Rôles
        foreach (var r in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(ClaimTypes.Role, r));

        // Permissions (claim custom)
        foreach (var p in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim("perm", p));

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: exp.UtcDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
