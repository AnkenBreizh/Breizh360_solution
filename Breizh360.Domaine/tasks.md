# Tâches — Domaine

> 2026-01-09

## USR (Users)

- `USR-DOM-001` — Entités Users (profil, invariants) — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Users/Entities/User.cs`
    - `Breizh360.Domaine/Users/ValueObjects/UserId.cs`
    - `Breizh360.Domaine/Users/ValueObjects/Email.cs`
    - `Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
- `USR-DOM-002` — Interfaces repos Users — ✅ **Fini (remis)**
  - **Remise :**
    - `Breizh360.Domaine/Users/Repositories/IUserRepository.cs`

## NOTIF (Notifications)

- `NOTIF-DOM-001` — (optionnel) Modèle inbox / persistance — ⏳ **À décider**
  - Dépendance : décision d’architecture (NOTIF persisté ou non)
  - Si validé : ajout entités + repository NOTIF + contrat `IF-NOTIF-001`
