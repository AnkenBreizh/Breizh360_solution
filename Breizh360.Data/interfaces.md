# Interfaces — Couche Données (Breizh360.Data)

Ce fichier documente les **contrats** exposés par la couche Données. Il sert de référence fonctionnelle
pour les autres équipes (Métier, API, Tests, Solution) et permet d'identifier les changements **cassants**.

> Source de vérité technique : le code dans `Breizh360.Data` et les migrations EF Core.

## Vue d'ensemble

La couche Données :

- **consomme** : `Breizh360.Domaine` (entités + interfaces de repositories)
- **produit** : implémentations EF Core de ces interfaces + schéma de base (migrations) + seed Dev

Les contrats ci-dessous sont alignés avec les tests contractuels de `Breizh360.Data.Tests`.

---

## IF_DATA_AUTH_001 — Schéma de persistance Auth

**But**

Garantir que le schéma EF Core du sous-domaine **Auth** reste stable (tables, relations, contraintes, indexes).

**Producteur** : `Breizh360.Data`

**Consommateurs** : `Breizh360.Metier`, `Breizh360.Api`, tests, outils d'administration

**Source de vérité**

- `Breizh360.Data/Migrations/Auth/*_AuthInitial.cs`
- `Breizh360.Data/Migrations/Auth/Breizh360DbContextModelSnapshot.cs`
- `Breizh360.Data/Auth/Configurations/*.cs`

**Règles de versioning**

- Toute modification de structure qui change la compatibilité (ajout de contrainte stricte, changement de type, renommage) est un **breaking change**.
- Les ajouts non bloquants (nouvelle colonne nullable, nouvel index) sont en général **compatibles**.

**Critères d'acceptation (fonctionnels)**

- Les entités Auth persistées couvrent : `User`, `Role`, `Permission`, `RefreshToken` et leurs jointures.
- Les relations many-to-many (User↔Role, Role↔Permission) sont matérialisées via des tables de jointure.
- Les clés/contraintes nécessaires au bon fonctionnement du domaine existent (unicité, FK, etc.).

---

## IF_DATA_AUTH_002 — Seed Dev Auth

**But**

Garantir un jeu de données **de développement** minimal et reproductible pour le sous-domaine Auth.

**Producteur** : `Breizh360.Data`

**Consommateurs** : développeurs, tests d'intégration, démonstrations

**Implémentation**

- `Breizh360.Data/Auth/Seed/AuthSeedDev.cs`

**Critères d'acceptation (fonctionnels)**

- Le seed peut être exécuté plusieurs fois sans casser la base (idempotence ou stratégie de "upsert"/détection).
- Le seed initialise au moins un compte administrateur et les rôles/permissions nécessaires aux scénarios de base.

---

## IF_AUTH_001 — Implémentations Data des repositories Auth

**But**

Garantir la disponibilité des implémentations EF Core des interfaces de repository du domaine (Auth).

**Producteur** : `Breizh360.Data`

**Consommateurs** : `Breizh360.Metier` (services d'authentification/autorisation), `Breizh360.Api`

**Interfaces domaine implémentées (côté `Breizh360.Domaine/Auth/...`)**

- `IUserRepository`
- `IRoleRepository`
- `IPermissionRepository`
- `IRefreshTokenRepository`

**Implémentations (côté `Breizh360.Data/Auth/Repositories/...`)**

- `UserRepository`
- `RoleRepository`
- `PermissionRepository`
- `RefreshTokenRepository`

**Exigences non fonctionnelles**

- Méthodes d'accès aux données **async** (quand pertinent) et support `CancellationToken` quand l'interface le prévoit.
- Pas de dépendance depuis `Breizh360.Domaine` vers `Breizh360.Data` (sens unique).
- Les repositories ne contiennent pas de logique métier ; ils gèrent la persistance et les requêtes.

**Points d’attention**

- *Tracking EF* : privilégier `AsNoTracking()` pour les lectures purement consultatives (si cohérent avec le domaine).
- *Transactions* : la frontière transactionnelle est portée par la couche applicative/métier (ou par l’hôte), pas par un repository isolé.
- *Migrations* : toute évolution du schéma doit être accompagnée d’une nouvelle migration + mise à jour des tests contractuels.

---

## Hors périmètre (pour l’instant)

- Câblage DI / composition root (API/Gateway/…)
- Stratégie CI/CD d'application des migrations
- Observabilité (logs/metrics) au niveau persistence (à définir globalement dans la solution)
