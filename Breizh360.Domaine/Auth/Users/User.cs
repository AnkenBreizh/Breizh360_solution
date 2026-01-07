using Breizh360.Domaine.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Breizh360.Domaine.Auth.Users;

public sealed class User : AuditEntity
{
    public const int LoginMaxLength = 64;
    public const int EmailMaxLength = 254;

    public string Login { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Hash du mot de passe (jamais le mot de passe en clair).
    /// Format à documenter dans docs/auth/01_modele_domaine.md (ex: PBKDF2$...).
    /// </summary>
    public string PasswordHash { get; private set; } = default!;

    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles;

    private User() { } // EF

    public User(string login, string email, string passwordHash, bool isActive = true)
    {
        SetLogin(login);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        IsActive = isActive;
    }

    public void SetLogin(string login)
    {
        var normalized = Normalization.NormalizeIdentityKey(login);

        if (normalized.Length is < 3 or > LoginMaxLength)
            throw new DomainException($"Login invalide (3..{LoginMaxLength}).");

        Login = normalized;
    }

    public void SetEmail(string email)
    {
        var normalized = Normalization.NormalizeIdentityKey(email);

        if (normalized.Length is < 5 or > EmailMaxLength)
            throw new DomainException($"Email invalide (5..{EmailMaxLength}).");

        // Validation "light"
        if (!normalized.Contains('@') || normalized.StartsWith('@') || normalized.EndsWith('@'))
            throw new DomainException("Email invalide (format).");

        Email = normalized;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SetPasswordHash(string passwordHash)
    {
        passwordHash = (passwordHash ?? string.Empty).Trim();

        if (passwordHash.Length < 20)
            throw new DomainException("PasswordHash invalide (trop court).");

        PasswordHash = passwordHash;
    }

    public void AddRole(Guid roleId)
    {
        if (roleId == Guid.Empty) throw new DomainException("RoleId invalide.");

        if (_roles.Any(r => r.RoleId == roleId))
            return;

        _roles.Add(new UserRole(Id, roleId));
    }

    public void RemoveRole(Guid roleId)
    {
        _roles.RemoveAll(r => r.RoleId == roleId);
    }
}
