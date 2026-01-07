using System.Security.Cryptography;
using System.Text;

namespace Breizh360.Metier.Auth;

/// <summary>
/// AUTH-BIZ-001 — Validation identifiants (hash + règles).
/// Remise attendue : Breizh360.Metier/Auth/AuthServiceValidateCredentials.cs
/// </summary>
public sealed class AuthServiceValidateCredentials
{
    // TODO: remplacer ces types par les interfaces/entités du Domaine
    private readonly dynamic _userRepository;

    public AuthServiceValidateCredentials(dynamic userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Valide login/email + mot de passe.
    /// </summary>
    public async Task<AuthServiceValidateCredentialsResult<dynamic>> ValidateAsync(
        string loginOrEmail,
        string password,
        CancellationToken ct)
    {
        // TODO: adapter méthode repo (ex: GetByLoginOrEmailAsync)
        var user = await _userRepository.GetByLoginOrEmailAsync(loginOrEmail, ct);

        if (user is null)
            return new(false, null);

        // TODO: si le Domaine porte soft-delete / disabled, appliquer ici
        // ex: if (user.IsDeleted || !user.IsActive) return new(false, null);

        // TODO: vérifier le hash avec la stratégie définie dans le Domaine
        // Ici, simple placeholder “hash compare” (à remplacer)
        var ok = SlowEquals(
            ComputeSha256(password),
            (string)user.PasswordHash
        );

        return new(ok, ok ? user : null);
    }

    // --- helpers (placeholder) ---
    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    private static bool SlowEquals(string a, string b)
    {
        var ba = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}
