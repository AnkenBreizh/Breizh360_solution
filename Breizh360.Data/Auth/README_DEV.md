# Auth (Data) — DEV : migrations & seed

## Pré-requis
- PostgreSQL accessible
- Connection string disponible via :
  - variable d’environnement : `ConnectionStrings__Postgres`
  - ou `ConnectionStrings:Postgres` (ex: `appsettings.Development.json` d’un projet de démarrage)

Le `Breizh360DbContextFactory` (DesignTime) essaie d’abord la variable d’environnement, puis un `appsettings.Development.json` situé dans un projet API au même niveau que la solution.

> Recommandé : dans le projet hôte (API/Worker), utilisez `builder.Services.AddBreizh360Data(builder.Configuration)`
> pour centraliser la configuration du DbContext et des repositories.

## EF Core — Migrations

Créer une migration (si vous en régénérez une) :
```bash
dotnet ef migrations add AuthInitial \
  --project Breizh360.Data \
  --context Breizh360DbContext \
  --output-dir Migrations/Auth
```

Appliquer les migrations :
```bash
dotnet ef database update \
  --project Breizh360.Data \
  --context Breizh360DbContext
```

> Si `dotnet ef` réclame un *startup project*, ajoutez `--startup-project <VotreProjet.Api>`.

## Seed DEV
Depuis votre projet de démarrage (API/Worker), après avoir résolu le `Breizh360DbContext` :

```csharp
await AuthSeedDev.EnsureSeedAsync(db, ct);
```

Seed généré :
- permissions : `auth.read`, `auth.write`, `auth.admin`
- rôles : `Admin`, `User`
- admin : `admin@local` (PasswordHash placeholder `DEV:CHANGE_ME`)
