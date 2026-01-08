using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Breizh360.Metier.Common;
using Microsoft.IdentityModel.Tokens;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-002 — Émission JWT + refresh token (hashé + rotation).
/// </summary>
public sealed class TokenService
{
    private readonly AuthOptionsJwt _jwt;
    private readonly IClock _clock;
    private readonly object? _refreshTokenRepository;

    public TokenService(AuthOptionsJwt jwt, IClock? clock = null, object? refreshTokenRepository = null)
    {
        _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        _clock = clock ?? new SystemClock();
        _refreshTokenRepository = refreshTokenRepository;
    }

    /// <summary>
    /// Émet une paire (access + refresh). Le refresh token est stocké côté DB uniquement sous forme de hash.
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

        var accessToken = CreateAccessToken(userId, login, roles, permissions, now, accessExp);

        // Refresh token opaque (jamais stocké en clair)
        var refreshToken = RefreshTokenCrypto.GenerateOpaqueToken();
        var refreshTokenHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);

        if (_refreshTokenRepository is not null)
        {
            await StoreRefreshTokenAsync(
                _refreshTokenRepository,
                userId,
                refreshTokenHash,
                now,
                refreshExp,
                ct
            ).ConfigureAwait(false);
        }

        return new TokenPair(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAtUtc: accessExp,
            RefreshTokenExpiresAtUtc: refreshExp
        );
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

        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Le refresh token est requis.", nameof(refreshToken));

        if (_refreshTokenRepository is null)
            throw new InvalidOperationException(
                "Le refresh token repository est requis pour effectuer un refresh (rotation). " +
                "Passez une implémentation via le constructeur TokenService(..., refreshTokenRepository: ...).");

        var now = _clock.UtcNow;
        var presentedHash = RefreshTokenCrypto.HashToken(refreshToken, _jwt.SigningKey);

        // Validation/consommation best-effort : on tente de supporter plusieurs signatures possibles.
        var isValid = await ValidateAndOptionallyConsumeRefreshTokenAsync(
            _refreshTokenRepository,
            userId,
            presentedHash,
            now,
            ct
        ).ConfigureAwait(false);

        if (!isValid)
            throw new SecurityTokenException("Refresh token invalide ou expiré.");

        // Rotation : émet une nouvelle paire + stocke le nouveau refresh hashé via StoreRefreshTokenAsync
        return await IssueAsync(
            userId,
            roles,
            permissions,
            login,
            ct
        ).ConfigureAwait(false);
    }

    private static async Task<bool> ValidateAndOptionallyConsumeRefreshTokenAsync(
        object refreshTokenRepository,
        Guid userId,
        string tokenHash,
        DateTimeOffset nowUtc,
        CancellationToken ct)
    {
        // Méthodes courantes de validation (retour bool)
        var validateNames = new[]
        {
            "ConsumeAsync",            // idéal : valide + révoque l'ancien token (one-time use)
            "ValidateAsync",
            "IsValidAsync",
            "ExistsAsync",
            "CheckAsync",
            "CanRefreshAsync"
        };

        var argCandidates = new List<object?[]>
        {
            // (userId, tokenHash)
            new object?[] { userId, tokenHash },
            // (userId, tokenHash, nowUtc)
            new object?[] { userId, tokenHash, nowUtc },
            // (tokenHash)
            new object?[] { tokenHash },
            // (tokenHash, nowUtc)
            new object?[] { tokenHash, nowUtc }
        };

        foreach (var args in argCandidates)
        {
            foreach (var name in validateNames)
            {
                var (found, result) = await RepoInvoke.TryCallAsync<bool>(
                    refreshTokenRepository,
                    name,
                    args,
                    ct
                ).ConfigureAwait(false);

                if (found)
                    return result;
            }
        }

        // Si aucune méthode bool n'existe, on ne peut pas garantir la validation côté service.
        throw new InvalidOperationException(
            "Aucune méthode compatible trouvée pour valider/consommer le refresh token. " +
            "Attendu : Consume/Validate/IsValid/Exists/Check/CanRefreshAsync(userId, tokenHash[, nowUtc][, ct]) " +
            "ou Consume/Validate/IsValid/Exists/Check/CanRefreshAsync(tokenHash[, nowUtc][, ct]).");
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

    private static async Task StoreRefreshTokenAsync(
        object refreshTokenRepository,
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc,
        CancellationToken ct)
    {
        // Best effort : supporter plusieurs signatures possibles (primitives).
        var methodNames = new[]
        {
            "CreateAsync",
            "AddAsync",
            "InsertAsync",
            "StoreAsync",
            "UpsertAsync",
            "SaveAsync"
        };

        var argCandidates = new List<object?[]>
        {
            // (userId, tokenHash, expiresAtUtc)
            new object?[] { userId, tokenHash, expiresAtUtc },
            // (userId, tokenHash, createdAtUtc, expiresAtUtc)
            new object?[] { userId, tokenHash, createdAtUtc, expiresAtUtc }
        };

        foreach (var args in argCandidates)
        {
            foreach (var name in methodNames)
            {
                var (found, _) = await RepoInvoke.TryCallAsync<object?>(
                    refreshTokenRepository,
                    name,
                    args,
                    ct
                ).ConfigureAwait(false);

                if (found)
                    return;
            }
        }

        // En dernier recours : une méthode qui prend un "record/entity" n'est pas invocable ici
        // sans référence compile-time. On force donc un message explicite.
        throw new InvalidOperationException(
            "Aucune méthode compatible trouvée pour stocker le refresh token. " +
            "Attendu : Create/Add/Insert/Store/Upsert/SaveAsync(userId, tokenHash, expiresAtUtc[, ct]) " +
            "ou Create/Add/Insert/Store/Upsert/SaveAsync(userId, tokenHash, createdAtUtc, expiresAtUtc[, ct]).");
    }
}
