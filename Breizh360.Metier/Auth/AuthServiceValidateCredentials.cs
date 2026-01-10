using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Breizh360.Domaine.Auth.Users;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-001 — Validation identifiants.
/// Objectif :
/// - normaliser login/email
/// - récupérer l'utilisateur via dépôt typé
/// - vérifier l'état verrouillé (future implémentation)
/// - comparer le mot de passe en temps constant
///
/// Cette version remplace l'ancienne implémentation basée sur la réflexion.
/// Elle dépend d'un repository fort typé <see cref="IAuthUserRepository"/> et
/// améliore la lisibilité ainsi que la sécurité de typage.
/// </summary>
public sealed class AuthServiceValidateCredentials
{
    private readonly IAuthUserRepository _userRepository;

    public AuthServiceValidateCredentials(IAuthUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Tentative de validation. Retourne IsValid=false sans révéler la raison
    /// (user inconnu vs mauvais mdp). Peut lever <see cref="AuthExceptionUserLocked"/>
    /// dans une implémentation future.
    /// </summary>
    public async Task<AuthServiceValidateCredentialsResult<User?>> TryValidateAsync(
        string loginOrEmail,
        string password,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(loginOrEmail) || string.IsNullOrEmpty(password))
            return new AuthServiceValidateCredentialsResult<User?>(false, null);

        // Normalisation : trim + lower invariant
        var key = NormalizeLoginOrEmail(loginOrEmail);

        // Recherche : d'abord login, puis email
        User? user = await _userRepository.GetByLoginAsync(key, ct).ConfigureAwait(false);
        if (user is null)
        {
            user = await _userRepository.GetByEmailAsync(key, ct).ConfigureAwait(false);
        }
        if (user is null)
            return new AuthServiceValidateCredentialsResult<User?>(false, null);

        // TODO: contrôle du verrouillage lorsqu'un champ LockedUntilUtc sera disponible sur User

        // Vérification du mot de passe. Le domaine expose uniquement PasswordHash
        // (formate PBKDF2 ou SHA256). Le helper ci-dessous supporte plusieurs formats.
        var storedHash = user.PasswordHash;
        if (string.IsNullOrWhiteSpace(storedHash))
            return new AuthServiceValidateCredentialsResult<User?>(false, null);

        bool ok = VerifyPassword(password, storedHash, null, null);
        if (!ok)
            return new AuthServiceValidateCredentialsResult<User?>(false, null);

        return new AuthServiceValidateCredentialsResult<User?>(true, user);
    }

    /// <summary>
    /// Variante stricte : lève <see cref="AuthExceptionInvalidCredentials"/>
    /// si les identifiants sont invalides.
    /// </summary>
    public async Task<User> ValidateOrThrowAsync(
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

    /// <summary>
    /// Supporte :
    /// - PBKDF2 : "PBKDF2$&lt;iterations&gt;$&lt;saltB64&gt;$&lt;hashB64&gt;"
    /// - PBKDF2 : "&lt;iterations&gt;.&lt;saltB64&gt;.&lt;hashB64&gt;"
    /// - Fallback (legacy/dev) : SHA256 hex (sans sel)
    /// </summary>
    private static bool VerifyPassword(string password, string storedHash, string? storedSalt, int? storedIterations)
    {
        // Format 1 : PBKDF2$100000$saltB64$hashB64
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
        // Format 3 : champs séparés (salt + hash + iterations)
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
        return Convert.ToHexString(bytes);
    }

    private static bool SlowEqualsHex(string a, string b)
    {
        a = a.Trim();
        b = b.Trim();
        var ba = Encoding.UTF8.GetBytes(a.ToUpperInvariant());
        var bb = Encoding.UTF8.GetBytes(b.ToUpperInvariant());
        return CryptographicOperations.FixedTimeEquals(ba, bb);
    }

    private static bool FixedTimeEquals(byte[] a, byte[] b)
        => CryptographicOperations.FixedTimeEquals(a, b);
}