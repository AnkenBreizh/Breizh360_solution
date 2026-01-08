# Interfaces exposées — Métier

> **Dernière mise à jour : 08 / 01 / 2026**


## IF-BIZ-AUTH-001 — Validation identifiants ( AuthServiceValidateCredentials )
- **Consommateurs :** API Métier, Tests
- **Contrat / exemples :**

Contrat : entrée loginOrEmail + password , renvoie AuthServiceValidateCredentialsResult<User> (succès + user) sans exposer si le login existe. Normalisation : utiliser Normalization.NormalizeIdentityKey côté Domaine avant requêtes. Erreurs : uniquement erreurs techniques (DB indispo) → exception ; credentials invalides → IsValid=false .

- **Remise :** Breizh360.Metier/Auth/AuthServiceValidateCredentials*.cs

## IF-BIZ-AUTH-002 — Émission/rotation tokens ( TokenService )
- **Consommateurs :** API Métier, Passerelle, Tests
- **Contrat / exemples :**

Contrat : IssueAsync génère access JWT + refresh opaque ; refresh stocké hashé (jamais en clair) ; RefreshAsync invalide l’ancien (rotation) et renvoie une nouvelle paire. Claims : sub =UserId, unique_name =login, role (multiples), perm (multiples), iat , jti . Erreurs : refresh invalide/expiré/révoqué → exception de sécurité (401 côté API).

- **Remise :** Breizh360.Metier/Auth/TokenService.cs + Breizh360.Metier/Auth/02_contrat_jwt.md

## IF-BIZ-AUTH-003 — Autorisation RBAC/ABAC ( AuthorizationServiceIsAllowed )
- **Consommateurs :** API Métier, Tests
- **Contrat / exemples :**

Contrat : IsAllowedAsync(userId, permission, ctx) → bool. RBAC (rôles→permissions) + ABAC optionnel via AuthorizationContext . Exemple : permission auth.admin ou règle ownership (suffixe :own ). Erreurs : règles métiers → false ; erreurs techniques → exception.

- **Remise :** Breizh360.Metier/Auth/AuthorizationServiceIsAllowed.cs
