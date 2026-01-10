using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Breizh360.Domaine.Auth.RefreshTokens;
using Breizh360.Metier.Common;
using Microsoft.IdentityModel.Tokens;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-002 — Émission JWT + refresh token (hashé + rotation).
///
/// Notes d'intégration :
/// - La persistance est faite via <see cref="IRefreshTokenRepository"/>.
/// - Le commit (SaveChanges/UoW) est à la charge de la couche appelante (API).
/// </summary>
public sealed class TokenService
{
    private readonly AuthOptionsJwt _jwt;
    private readonly IClock _clock;
    private readonly IRefreshTokenRepository? _refreshTokenRepository;

    /// <summary>
    /// Constructeur principal (signature historique). Le dépôt est attendu sous forme de
    /// <see cref="IRefreshTokenRepository"/> (le paramètre <c>object</c> est conservé pour
    /// compatibilité avec l'existant).
    /// </summary>
    public TokenService(AuthOptionsJwt jwt, IClock? clock = null, object? refreshTokenRepository = null)
    {
        _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        _clock = clock ?? new SystemClock();
        _refreshTokenRepository = refreshTokenRepository as IRefreshTokenRepository
            ?? (refreshTokenRepository is null
                ? null
                : throw new ArgumentException(
                    "Le refreshTokenRepository doit implémenter IRefreshTokenRepository (Breizh360.Domaine.Auth.RefreshTokens).",
                    nameof(refreshTokenRepository)));
    }

    /// <summary>
    /// Overload typé (facilite l'injection DI).
    /// </summary>
    public TokenService(AuthOptionsJwt jwt, IRefreshTokenRepository refreshTokenRepository, IClock? clock = null)
        : this(jwt, clock, refreshTokenRepository)
    {
    }

    /// <summary>
    /// Émet une paire (access + refresh). Le refresh token n'est jamais stocké en clair.
    /// </summary>
    public async Task<TokenPair> IssueAsync(
        Guid userId,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null,
        string? login = null,
        CancellationToken ct = default)
    {
        ValidateJwtOptions(_jwt);

        var now = _clock.UtcNow;
        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var refreshExp = now.AddDays(_jwt.RefreshTokenDays);

        var issue = BuildIssue(userId, login, roles, permissions, now, accessExp, refreshExp);

        if (_refreshTokenRepository is not null)
        {
            // Persist uniquement le hash + métadonnées
            issue.RefreshTokenEntity.MarkCreated(actorId: userId, now: now);
            await _refreshTokenRepository.AddAsync(issue.RefreshTokenEntity, ct).ConfigureAwait(false);
        }

        return issue.Pair;
    }

    /// <summary>
    /// Rafraîchit une paire (access + refresh) en validant le refresh token fourni puis en effectuant une rotation.
    /// </summary>
    public async Task<TokenPair> RefreshAsync(
        Guid userId,
        string refreshToken,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null,
        string? login = null,
        CancellationToken ct = default)
    {
        ValidateJwtOptions(_jwt);

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId invalide.", nameof(userId));
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Le refresh token est requis.", nameof(refreshToken));

        if (_refreshTokenRepository is null)
            throw new InvalidOperationException(
                "Le refresh token repository est requis pour effectuer un refresh (rotation). " +
                "Passez une implémentation via le constructeur TokenService(..., refreshTokenRepository: ...)."
            );

        var now = _clock.UtcNow;
        var presentedHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);

        var current = await _refreshTokenRepository.GetByTokenHashAsync(presentedHash, ct).ConfigureAwait(false);
        if (current is null)
            throw new SecurityTokenException("Refresh token invalide.");

        if (current.UserId != userId)
            throw new SecurityTokenException("Refresh token invalide.");

        // Validation côté service (évite de dépendre d'une propriété calculée basée sur DateTimeOffset.UtcNow)
        if (current.IsDeleted || current.RevokedAt is not null || current.ExpiresAt <= now)
            throw new SecurityTokenException("Refresh token expiré ou révoqué.");

        // Nouvelle paire (rotation)
        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var refreshExp = now.AddDays(_jwt.RefreshTokenDays);
        var issue = BuildIssue(userId, login, roles, permissions, now, accessExp, refreshExp);

        // Lier + révoquer l'ancien token
        current.ReplaceBy(issue.RefreshTokenEntity.Id, actorId: userId, now: now);
        current.Revoke(actorId: userId, reason: "rotated", now: now);

        await _refreshTokenRepository.UpdateAsync(current, ct).ConfigureAwait(false);

        issue.RefreshTokenEntity.MarkCreated(actorId: userId, now: now);
        await _refreshTokenRepository.AddAsync(issue.RefreshTokenEntity, ct).ConfigureAwait(false);

        return issue.Pair;
    }

    // -----------------------
    // Internals
    // -----------------------

    private sealed record IssueResult(TokenPair Pair, RefreshToken RefreshTokenEntity);

    private IssueResult BuildIssue(
        Guid userId,
        string? login,
        IEnumerable<string>? roles,
        IEnumerable<string>? permissions,
        DateTimeOffset now,
        DateTimeOffset accessExp,
        DateTimeOffset refreshExp)
    {
        var accessToken = CreateAccessToken(userId, login, roles, permissions, now, accessExp);

        // Refresh token opaque (jamais stocké en clair)
        var refreshToken = RefreshTokenCrypto.GenerateOpaqueToken();
        var refreshTokenHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);

        var entity = new RefreshToken(userId, refreshTokenHash, refreshExp);

        var pair = new TokenPair(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAtUtc: accessExp,
            RefreshTokenExpiresAtUtc: refreshExp
        );

        return new IssueResult(pair, entity);
    }

    private string CreateAccessToken(
        Guid userId,
        string? login,
        IEnumerable<string>? roles,
        IEnumerable<string>? permissions,
        DateTimeOffset now,
        DateTimeOffset exp)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString("D")),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrWhiteSpace(login))
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, login));

        if (roles is not null)
        {
            foreach (var r in roles.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
                claims.Add(new Claim(ClaimTypes.Role, r));
        }

        if (permissions is not null)
        {
            foreach (var p in permissions.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
                claims.Add(new Claim("perm", p));
        }

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

    private static void ValidateJwtOptions(AuthOptionsJwt jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt.SigningKey))
            throw new InvalidOperationException("AuthOptionsJwt.SigningKey est requis.");
        if (Encoding.UTF8.GetByteCount(jwt.SigningKey) < 32)
            throw new InvalidOperationException("AuthOptionsJwt.SigningKey doit faire au moins 32 octets (256 bits) en UTF-8.");
        if (string.IsNullOrWhiteSpace(jwt.Issuer))
            throw new InvalidOperationException("AuthOptionsJwt.Issuer est requis.");
        if (string.IsNullOrWhiteSpace(jwt.Audience))
            throw new InvalidOperationException("AuthOptionsJwt.Audience est requis.");
        if (jwt.AccessTokenMinutes <= 0)
            throw new InvalidOperationException("AuthOptionsJwt.AccessTokenMinutes doit être > 0.");
        if (jwt.RefreshTokenDays <= 0)
            throw new InvalidOperationException("AuthOptionsJwt.RefreshTokenDays doit être > 0.");
    }
}
