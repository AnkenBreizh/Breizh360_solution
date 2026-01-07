using Breizh360.Domaine.Common;

namespace Breizh360.Domaine.Auth.Permissions;

public sealed class Permission : AuditEntity
{
    public const int CodeMaxLength = 128;

    /// <summary>
    /// Code stable, versionné par convention si besoin.
    /// Ex: users.read, users.write, admin.*, auth.refresh
    /// </summary>
    public string Code { get; private set; } = default!;

    public string? Description { get; private set; }

    private Permission() { } // EF

    public Permission(string code, string? description = null)
    {
        SetCode(code);
        SetDescription(description);
    }

    public void SetCode(string code)
    {
        code = Normalization.NormalizeIdentityKey(code);

        if (code.Length is < 3 or > CodeMaxLength)
            throw new DomainException($"Permission.Code invalide (3..{CodeMaxLength}).");

        // Garde-fou : pas d'espaces
        foreach (var ch in code)
        {
            if (char.IsWhiteSpace(ch))
                throw new DomainException("Permission.Code invalide (espaces interdits).");
        }

        Code = code;
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
