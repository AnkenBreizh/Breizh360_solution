# Demandes inter-équipes — Breizh360

> **Dernière mise à jour :** 2026-01-09  
> **Source :** https://github.com/AnkenBreizh/Breizh360_solution

## Modèle (copier/coller)
### `XXX-REQ-000` — [TITRE]
- **De :** …
- **À :** …
- **Nécessaire pour :** Feature (`AUTH`/`USR`/`NOTIF`) / `INIT-XXX`
- **Détails :**
  - …
- **Remise attendue :**
  - `chemin/vers/doc.md` ou PR ou fichier
- **Statut :** Demande | En cours | Terminé

---

## Demandes — USR (Users)

### `USR-REQ-001` — Contrat Domaine Users (entités + repos)
- **De :** API Métier
- **À :** Domaine
- **Nécessaire pour :** USR
- **Détails :**
  - Entités UserProfile/Preferences (si prévu), invariants (email unique, etc.)
  - Interfaces repos (IUserRepository, IRoleRepository si réutilisé, etc.)
- **Remise attendue :**
  - `/Breizh360.Domaine/interfaces.md` (sections IF-USR-…)
- **Statut :** Demande

### `USR-REQ-002` — Contrat API Users (DTO + erreurs + exemples)
- **De :** UI
- **À :** API Métier
- **Nécessaire pour :** USR
- **Remise attendue :**
  - `/Breizh360.Api.Metier/interfaces.md` (IF-API-USR-…)
- **Statut :** Demande

## Demandes — NOTIF (Notifications)

### `NOTIF-REQ-001` — Contrat hubs/événements (noms + payloads)
- **De :** UI
- **À :** API Métier
- **Nécessaire pour :** NOTIF
- **Détails :**
  - Noms hubs, méthodes, événements poussés, schémas payload
- **Remise attendue :**
  - `/Breizh360.Api.Metier/interfaces.md` (IF-API-NOTIF-…)
- **Statut :** Demande

### `NOTIF-REQ-002` — Proxy Gateway /hubs + WebSockets (validation)
- **De :** API Métier
- **À :** Passerelle
- **Nécessaire pour :** NOTIF
- **Remise attendue :**
  - `/Breizh360.Gateway/interfaces.md` (IF-GATE-NOTIF-…)
- **Statut :** Demande
