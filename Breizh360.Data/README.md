# Breizh360.Data — Couche Données (EF Core)

Ce module fournit la couche de persistance de la solution **Breizh360** via **Entity Framework Core**.
Il héberge :

- le `DbContext` (`Breizh360DbContext`) ;
- les configurations EF (`IEntityTypeConfiguration<>`) ;
- les migrations (notamment **Auth**) ;
- les *seeds* de développement ;
- les implémentations des *repositories* définis côté `Breizh360.Domaine`.

## Périmètre actuel

### Auth

La partie Auth est structurée autour de :

- `Auth/Configurations` : mapping EF des entités Auth (User/Role/Permission/…)
- `Auth/Repositories` : implémentations EF des interfaces de repository du domaine
- `Auth/Seed` : seed *Dev* (jeu de données de départ)
- `Migrations/Auth` : migration initiale + snapshot du modèle Auth

## Structure du projet (repères)

- `Breizh360DbContext.cs` : point d’entrée EF (DbSets + configuration)
- `DesignTime/Breizh360DbContextFactory.cs` : factory *design-time* pour `dotnet ef`
- `Common/EntityEntryExtensions.cs` : helpers EF (ex: audit/updates)

## Démarrage rapide (développeur)

1. **Configurer une chaîne de connexion** pour EF Core (selon l’application hôte : API, tests, outil console, etc.).
2. **Appliquer la migration Auth**.
   - La source de vérité du schéma est la migration `Migrations/Auth/*_AuthInitial.cs`.
3. **(Optionnel) Seed Dev**
   - Utiliser `Auth/Seed/AuthSeedDev.cs` pour initialiser un jeu de données de dev (utilisateurs/roles/permissions).

> Remarque : le câblage DI (enregistrement du DbContext + repositories) est fait dans l’application hôte —
> idéalement via l’extension <c>AddBreizh360Data(...)</c> ci-dessous.

## Intégration (DI) — recommandé

Le module expose un point d’entrée DI pour standardiser l’intégration depuis la *composition root*
(API, Worker, Gateway...).

Dans votre projet hôte :

```csharp
using Breizh360.Data;

builder.Services.AddBreizh360Data(builder.Configuration);
```

Par défaut, l’extension lit la connection string **`ConnectionStrings:Postgres`**
(ou variable d’environnement `ConnectionStrings__Postgres`).

Options (ex: DEV) :

```csharp
builder.Services.AddBreizh360Data(builder.Configuration, o =>
{
  o.EnableDetailedErrors = true;
  o.EnableSensitiveDataLogging = true;
});
```

## Contrats / Interfaces (références)

Les contrats de la couche Données sont documentés dans `interfaces.md` :

- **IF_DATA_AUTH_001** — Schéma de persistance Auth (EF Core)
- **IF_DATA_AUTH_002** — Seed Dev Auth
- **IF_AUTH_001** — Implémentations des repositories Auth

## Dépendances

- Dépend de `Breizh360.Domaine` (entités + interfaces)
- Fournit des implémentations EF Core consommées par `Breizh360.Metier` et/ou `Breizh360.Api`
