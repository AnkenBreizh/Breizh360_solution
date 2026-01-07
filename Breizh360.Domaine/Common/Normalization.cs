using System;

namespace Breizh360.Domaine.Common;

public static class Normalization
{
    /// <summary>
    /// Normalise une clé d'identité (login/email) pour les recherches et l'unicité.
    /// Contrainte: Trim + ToLowerInvariant.
    /// </summary>
    public static string NormalizeIdentityKey(string value)
    {
        value = (value ?? string.Empty).Trim();

        if (value.Length == 0)
            throw new DomainException("Valeur vide (normalisation).");

        return value.ToLowerInvariant();
    }
}
