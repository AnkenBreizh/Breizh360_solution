# Domaine — Breizh360

> 2026-01-09

## Rôle

Le projet **Breizh360.Domaine** contient :
- Le **modèle de domaine** (entités, value objects, invariants)
- Les **contrats de persistance** côté domaine (interfaces de repositories)

## Structure (attendue)

- `Breizh360.Domaine/Common/...`
- `Breizh360.Domaine/Users/...`

## Statut

- ✅ `USR-DOM-001` — Entités Users (profil, invariants) — **remis**
- ✅ `USR-DOM-002` — Interfaces repos Users — **remis**
- ⏳ `NOTIF` — (optionnel) en attente de décision (persistance inbox)

## Conventions d’équipe

- **ID stable obligatoire** (tasks + interfaces).
- **Contrats avant implémentation** : le fichier `interfaces.md` fait foi.
- **Fini = Remise** : une tâche n’est “done” que si les fichiers attendus sont présents.

## Remise (fichiers livrés)

- `Breizh360.Domaine/Common/DomainException.cs`
- `Breizh360.Domaine/Users/Entities/User.cs`
- `Breizh360.Domaine/Users/ValueObjects/UserId.cs`
- `Breizh360.Domaine/Users/ValueObjects/Email.cs`
- `Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
- `Breizh360.Domaine/Users/Repositories/IUserRepository.cs`
