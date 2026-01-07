# Auth — Modèle de domaine

## Objectif
Décrire le modèle de domaine **Auth** (entités + invariants) afin d’aligner Domaine / Métier / Data / API.

---

## Conventions transverses

### Normalisation (Login / Email / Names / Codes)
Pour éviter les doublons “case sensitive”, les clés d’identité sont normalisées par convention :

- `Trim()` puis `ToLowerInvariant()`
- implémenté par `Breizh360.Domaine.Common.Normalization.NormalizeIdentityKey(...)`

> Les repositories `GetByLoginAsync` / `GetByEmailAsync` attendent des valeurs **déjà normalisées**.

### Audit & Soft delete
Toutes les entités principales héritent de `AuditEntity` :

- `Id : Guid`
- Audit : `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Soft delete : `IsDeleted`, `DeletedAt`, `DeletedBy`

**Règle** : la suppression logique se fait via `entity.SoftDelete(...)`.  
Le filtre global `IsDeleted = false` est géré côté **Data** (EF Core), pas dans le Domaine.

---

## Entités

### User (`Breizh360.Domaine.Auth.Users.User`)
Champs :
- `Login` (normalisé, unique) — longueur 3..64
- `Email` (normalisé, unique) — longueur 5..254
- `IsActive`
- `PasswordHash` (jamais de mot de passe en clair)
- `Roles` : association via `UserRole (UserId, RoleId)`

Invariants principaux :
- login/email non vides + normalisés
- validation email “light” (`@` présent, pas au début/à la fin)
- `PasswordHash` longueur minimale (garde-fou)

### Role (`Breizh360.Domaine.Auth.Roles.Role`)
Champs :
- `Name` (normalisé, unique)
- `Description` (optionnel)
- `Permissions` : association via `RolePermission (RoleId, PermissionId)`

### Permission (`Breizh360.Domaine.Auth.Permissions.Permission`)
Champs :
- `Code` (normalisé, unique) — ex : `users.read`, `users.write`, `admin.*`, `auth.refresh`
- `Description` (optionnel)

Règle : pas d’espaces dans `Code`.

### RefreshToken (`Breizh360.Domaine.Auth.RefreshTokens.RefreshToken`)
Champs :
- `UserId`
- `TokenHash` (jamais le token en clair)
- `ExpiresAt`
- Révocation : `RevokedAt`, `RevokedBy`, `RevokeReason`
- Rotation : `ReplacedByTokenId`

Propriétés calculées :
- `IsExpired`, `IsRevoked`, `IsActive`

---

## Hash mot de passe

Le mot de passe n’est jamais stocké en clair.  
Le domaine stocke une chaîne `PasswordHash` versionnée.

### Format recommandé
`PBKDF2$<iterations>$<saltBase64>$<hashBase64>`

- Algo : PBKDF2-HMAC-SHA256
- `iterations` : ex 100_000 (à ajuster selon perf)
- `salt` : 16 octets min (aléatoire)
- `hash` : 32 octets min

> Le calcul du hash est implémenté côté **Métier/Infra** (pas dans le Domaine).

---

## Refresh token (professionnel)

### Token côté client
- Token opaque aléatoire (au moins 32 octets de random)
- Stockage client sécurisé (ex : cookie HttpOnly/SameSite pour web)

### Stockage côté serveur : HMAC-SHA-256 (pepper)
On stocke uniquement une **MAC** du token avec une clé serveur :

`HMACSHA256$kid=<keyId>$<base64url(mac)>`

- `kid` : identifiant de clé (rotation des secrets)
- `mac` : HMAC-SHA-256(token_bytes, secret_key_for_kid)

Validation :
- recalcul de la MAC + comparaison en **constant-time**

Rotation :
- à chaque refresh : émettre un nouveau refresh token et révoquer/relier l’ancien via `ReplacedByTokenId`

---

## RBAC / ABAC

### RBAC (Role-Based Access Control)
- Un `User` a des `Roles` (via `UserRole`)
- Un `Role` a des `Permissions` (via `RolePermission`)
- L’accès à une action se fait via la présence d’une permission donnée.

### ABAC (Attribute-Based Access Control)
ABAC n’est pas modélisé en “règles” dans le Domaine.  
Il est évalué côté **Métier** via attributs (ex : propriété, organisation, contexte) en complément des permissions.

---

## Contrats de dépôts (interfaces)
Interfaces minimales côté Domaine :

- `IUserRepository`
- `IRoleRepository`
- `IPermissionRepository`
- `IRefreshTokenRepository`

> L’implémentation est côté **Data** (EF Core).
