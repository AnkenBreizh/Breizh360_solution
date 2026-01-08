# Interfaces exposées — Domaine

> **Dernière mise à jour : 08 / 01 / 2026**


## IF-AUTH-001 — Contrats de dépôts Auth ( IUserRepository , IRoleRepository , IPermissionRepository , IRefreshTokenRepository )
- **Consommateurs :** Données, Métier, API
- **Contrat / exemples :**

Contrat : méthodes async, recherche par clés normalisées (login/email/nom rôle) ; soft delete via SoftDeleteAsync (users/roles/permissions) ; refresh token uniquement via hash (jamais en clair). Exemple : var key = Normalization.NormalizeIdentityKey(login); var user = await userRepo.GetByLoginAsync(key); Erreurs : DomainException si normalisation impossible (valeur vide). Les méthodes Get* retournent null si non trouvé.

- **Remise :** Breizh360.Domaine/Auth/*/*.cs

## IF-COMMON-001 — Socle Domaine ( AuditEntity + soft delete)
- **Consommateurs :** Données, Métier, API
- **Contrat / exemples :**

Contrat : audit ( CreatedAt/By , UpdatedAt/By ) + soft delete ( IsDeleted , DeletedAt/By ) via MarkCreated / MarkUpdated / SoftDelete / Restore . Exemple : entity.MarkCreated(actorId); puis côté Data : filtre global IsDeleted = false . Erreurs : invariants à respecter côté Métier (ex : ne pas “restaurer” un élément non supprimé, selon règles de service).

- **Remise :** Breizh360.Domaine/Common/*.cs
