# Suivi — Équipe Données (Breizh360.Data)

Dernière mise à jour : **2026-01-09**

## État global

### ✅ Terminé

- Mise en place du projet `Breizh360.Data` + `Breizh360DbContext` + factory design-time.
- Mapping EF Core du sous-domaine **Auth** (Configurations EF : Users/Roles/Permissions/RefreshTokens + tables de jointure).
- Implémentation des repositories Auth (EF Core) qui s'appuient sur le domaine.
- Migration initiale Auth + snapshot du modèle.
- Seed Dev Auth.
- Tests contractuels ciblés sur le schéma Auth, le seed Dev et les repositories.

### 🟡 En cours

- (Rien en cours côté Data à cette étape.)

### ⏳ À faire / Points de vigilance

- Vérifier l’intégration DI côté application hôte (API/Gateway/…): enregistrement du DbContext et des repositories.
- Valider la stratégie de migrations (provider, pipeline CI, conventions de nommage si la solution se standardise).
- Compléter l’index global des interfaces (`Docs/interfaces_index.md`) si le Responsable Solution ne l’a pas déjà fait.

## Backlog détaillé

| ID | Sujet | Statut | Livrable / chemin attendu | Notes |
|---:|---|:---:|---|---|
| DATA-001 | Créer le projet `Breizh360.Data` + références | ✅ | `Breizh360.Data/Breizh360.Data.csproj` | Base de la couche persistence |
| DATA-002 | Ajouter `Breizh360DbContext` | ✅ | `Breizh360.Data/Breizh360DbContext.cs` | DbSets + ApplyConfigurations |
| DATA-003 | Factory design-time pour `dotnet ef` | ✅ | `Breizh360.Data/DesignTime/Breizh360DbContextFactory.cs` | Migrations en local/CI |
| DATA-010 | EF Config — Users | ✅ | `Breizh360.Data/Auth/Configurations/UserEfConfiguration.cs` | Contraintes/index à valider via tests |
| DATA-011 | EF Config — Roles | ✅ | `Breizh360.Data/Auth/Configurations/RoleEfConfiguration.cs` |  |
| DATA-012 | EF Config — Permissions | ✅ | `Breizh360.Data/Auth/Configurations/PermissionEfConfiguration.cs` |  |
| DATA-013 | EF Config — Jointures (UserRole, RolePermission) | ✅ | `.../UserRoleEfConfiguration.cs`, `.../RolePermissionEfConfiguration.cs` |  |
| DATA-014 | EF Config — RefreshTokens | ✅ | `Breizh360.Data/Auth/Configurations/RefreshTokenEfConfiguration.cs` |  |
| DATA-020 | Repositories — Users | ✅ | `Breizh360.Data/Auth/Repositories/UserRepository.cs` | Impl. de l’interface domaine |
| DATA-021 | Repositories — Roles | ✅ | `Breizh360.Data/Auth/Repositories/RoleRepository.cs` |  |
| DATA-022 | Repositories — Permissions | ✅ | `Breizh360.Data/Auth/Repositories/PermissionRepository.cs` |  |
| DATA-023 | Repositories — RefreshTokens | ✅ | `Breizh360.Data/Auth/Repositories/RefreshTokenRepository.cs` |  |
| DATA-030 | Migration initiale Auth + snapshot | ✅ | `Breizh360.Data/Migrations/Auth/*_AuthInitial.cs` + `DbContextModelSnapshot.cs` | Source de vérité du schéma |
| DATA-040 | Seed Dev Auth | ✅ | `Breizh360.Data/Auth/Seed/AuthSeedDev.cs` | Données de dev (admin/roles/permissions...) |
| DATA-050 | Contrat IF_DATA_AUTH_001 — schéma Auth | ✅ | `Breizh360.Data.Tests/*IF_DATA_AUTH_001*` | Contrat = schéma stable |
| DATA-051 | Contrat IF_DATA_AUTH_002 — seed Dev | ✅ | `Breizh360.Data.Tests/*IF_DATA_AUTH_002*` | Contrat = jeu de seed |
| DATA-052 | Contrat IF_AUTH_001 — repositories impl. | ✅ | `Breizh360.Data.Tests/*IF_AUTH_001*` | Vérifie la présence/implémentation |
| DATA-090 | Câblage DI côté hôte (API/…) | ⏳ | (dans le projet hôte) | Hors Data pur, mais à suivre |
| DATA-091 | Index global des interfaces | ⏳ | `Docs/interfaces_index.md` | À valider avec Responsable Solution |
