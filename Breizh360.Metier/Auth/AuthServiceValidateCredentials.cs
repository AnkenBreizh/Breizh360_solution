using System.Security.Cryptography;
using System.Text;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-001 — Validation identifiants.
/// Objectif :
/// - normaliser login/email
/// - récupérer l'utilisateur via dépôt
/// - vérifier l'état verrouillé
/// - comparer le mot de passe en temps constant
///
/// Notes :
/// - Le Domaine doit rester source de vérité sur le format exact du hash.
/// - Ce service supporte plusieurs formats "compat" (PBKDF2 + fallback SHA256) pour éviter de bloquer l'intégration.
/// </summary>
public sealed class AuthServiceValidateCredentials
{
    private readonly object _userRepository;

    public AuthServiceValidateCredentials(object userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Tentative de validation. Retourne IsValid=false sans révéler la raison (user inconnu vs mauvais mdp).
    /// Peut lever <see cref="AuthExceptionUserLocked"/> si le compte est verrouillé.
    /// </summary>
    public async Task<AuthServiceValidateCredentialsResult<dynamic>> TryValidateAsync(
        string loginOrEmail,
        string password,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(loginOrEmail) || string.IsNullOrEmpty(password))
            return new AuthServiceValidateCredentialsResult<dynamic>(false, null);

        var key = NormalizeLoginOrEmail(loginOrEmail);

        // 1) Récupérer user (meilleur effort sur les signatures courantes)
        dynamic? user = await RepoInvoke.CallFirstAsync<dynamic>(
            _userRepository,
            new[]
            {
                "GetByLoginOrEmailAsync",
                "FindByLoginOrEmailAsync",
                "GetByIdentifierAsync",
                "FindByIdentifierAsync"
            },
            new object?[] { key },
            ct
        ).ConfigureAwait(false);

        if (user is null)
        {
            // fallback : GetByLoginAsync puis GetByEmailAsync
            user = await RepoInvoke.CallFirstAsync<dynamic>(
                _userRepository,
                new[] { "GetByLoginAsync", "FindByLoginAsync" },
                new object?[] { key },
                ct
            ).ConfigureAwait(false);

            user ??= await RepoInvoke.CallFirstAsync<dynamic>(
                _userRepository,
                new[] { "GetByEmailAsync", "FindByEmailAsync" },
                new object?[] { key },
                ct
            ).ConfigureAwait(false);
        }

        if (user is null)
            return new AuthServiceValidateCredentialsResult<dynamic>(false, null);

        // 2) Verrouillage compte
        if (TryGetLockedUntilUtc(user, out DateTimeOffset? lockedUntilUtc) && lockedUntilUtc is not null && lockedUntilUtc > DateTimeOffset.UtcNow)
            throw new AuthExceptionUserLocked(lockedUntilUtc);

        // 3) Hash stocké + vérification
        var storedHash = GetStringProperty(user, new[] { "PasswordHash", "PasswordHashHex", "PasswordHashValue" });
        var storedSalt = GetStringProperty(user, new[] { "PasswordSalt", "Salt", "PasswordSaltBase64" });
        var iterations = GetIntProperty(user, new[] { "PasswordIterations", "Iterations" });

        if (string.IsNullOrWhiteSpace(storedHash))
            return new AuthServiceValidateCredentialsResult<dynamic>(false, null);

        var ok = VerifyPassword(password, storedHash!, storedSalt, iterations);
        if (!ok)
            return new AuthServiceValidateCredentialsResult<dynamic>(false, null);

        return new AuthServiceValidateCredentialsResult<dynamic>(true, user);
    }

    /// <summary>
    /// Variante stricte : lève <see cref="AuthExceptionInvalidCredentials"/> si invalides.
    /// </summary>
    public async Task<dynamic> ValidateOrThrowAsync(
        string loginOrEmail,
        string password,
        CancellationToken ct = default)
    {
        var r = await TryValidateAsync(loginOrEmail, password, ct).ConfigureAwait(false);
        if (!r.IsValid || r.User is null)
            throw new AuthExceptionInvalidCredentials();
        return r.User;
    }

    // -----------------------
    // Helpers
    // -----------------------

    private static string NormalizeLoginOrEmail(string input)
        => input.Trim().ToLowerInvariant();

    private static bool TryGetLockedUntilUtc(dynamic user, out DateTimeOffset? lockedUntilUtc)
    {
        lockedUntilUtc = null;
        if (user is null) return false;

        // common property names
        DateTimeOffset? dto;
        if (RepoInvoke.TryGetProperty<DateTimeOffset?>(user, new[] { "LockedUntilUtc", "LockoutEndUtc" }, out dto))
        {
            lockedUntilUtc = dto;
            return true;
        }

        // Sometimes stored as DateTime
        DateTime? dt;
        if (RepoInvoke.TryGetProperty<DateTime?>(user, new[] { "LockedUntilUtc", "LockoutEndUtc" }, out dt) && dt.HasValue)
        {
            lockedUntilUtc = new DateTimeOffset(DateTime.SpecifyKind(dt!.Value, DateTimeKind.Utc));
            return true;
        }

        return false;
    }

    private static string? GetStringProperty(dynamic obj, IEnumerable<string> names)
    {
        string? v;
        return RepoInvoke.TryGetProperty<string>(obj, names, out v) ? v : null;
    }

    private static int? GetIntProperty(dynamic obj, IEnumerable<string> names)
    {
        int v;
        return RepoInvoke.TryGetProperty<int>(obj, names, out v) ? v : (int?)null;
    }

    /// <summary>
    /// Supporte :
    /// - PBKDF2 : "PBKDF2$&lt;iterations&gt;$&lt;saltB64&gt;$&lt;hashB64&gt;"
    /// - PBKDF2 : "&lt;iterations&gt;.&lt;saltB64&gt;.&lt;hashB64&gt;"
    /// - Fallback (legacy/dev) : SHA256 hex (sans sel)
    /// </summary>
    private static bool VerifyPassword(string password, string storedHash, string? storedSalt, int? storedIterations)
    {
        // Format 1 : PBKDF2$iter$salt$hash
        if (TryParsePbkdf2Dollar(storedHash, out var it1, out var salt1, out var hash1))
        {
            var computed = Pbkdf2(password, salt1, it1, hash1.Length);
            return FixedTimeEquals(hash1, computed);
        }

        // Format 2 : iter.salt.hash
        if (TryParsePbkdf2Dot(storedHash, out var it2, out var salt2, out var hash2))
        {
            var computed = Pbkdf2(password, salt2, it2, hash2.Length);
            return FixedTimeEquals(hash2, computed);
        }

        // Format 3 : salt + hash (separate fields) if available
        if (!string.IsNullOrWhiteSpace(storedSalt) && storedIterations is not null)
        {
            if (TryFromBase64(storedSalt!, out var saltB))
            {
                // storedHash could be base64 or hex
                if (TryFromBase64(storedHash, out var hashB))
                {
                    var computed = Pbkdf2(password, saltB, storedIterations.Value, hashB.Length);
                    return FixedTimeEquals(hashB, computed);
                }
                if (TryFromHex(storedHash, out var hashHex))
                {
                    var computed = Pbkdf2(password, saltB, storedIterations.Value, hashHex.Length);
                    return FixedTimeEquals(hashHex, computed);
                }
            }
        }

        // Fallback : SHA256 hex
        var computedHex = ComputeSha256Hex(password);
        return SlowEqualsHex(computedHex, storedHash);
    }

    private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        => Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: Math.Max(iterations, 10_000),
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: outputBytes);

    private static bool TryParsePbkdf2Dollar(string s, out int iterations, out byte[] salt, out byte[] hash)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        hash = Array.Empty<byte>();

        // PBKDF2$100000$saltB64$hashB64
        var parts = s.Split('$', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 4) return false;
        if (!parts[0].Equals("PBKDF2", StringComparison.OrdinalIgnoreCase)) return false;
        if (!int.TryParse(parts[1], out iterations)) return false;
        if (!TryFromBase64(parts[2], out salt)) return false;
        if (!TryFromBase64(parts[3], out hash)) return false;
        return true;
    }

    private static bool TryParsePbkdf2Dot(string s, out int iterations, out byte[] salt, out byte[] hash)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        hash = Array.Empty<byte>();

        var parts = s.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out iterations)) return false;
        if (!TryFromBase64(parts[1], out salt)) return false;
        if (!TryFromBase64(parts[2], out hash)) return false;
        return true;
    }

    private static bool TryFromBase64(string s, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(s);
            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    private static bool TryFromHex(string s, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromHexString(s);
            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    private static string ComputeSha256Hex(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes); // uppercase
    }

    private static bool SlowEqualsHex(string a, string b)
    {
        // Normalize hex strings (ignore case + whitespace)
        a = a.Trim();
        b = b.Trim();

        // If b is not hex, compare as UTF8
        var ba = Encoding.UTF8.GetBytes(a.ToUpperInvariant());
        var bb = Encoding.UTF8.GetBytes(b.ToUpperInvariant());
        return CryptographicOperations.FixedTimeEquals(ba, bb);
    }

    private static bool FixedTimeEquals(byte[] a, byte[] b)
        => CryptographicOperations.FixedTimeEquals(a, b);
}
