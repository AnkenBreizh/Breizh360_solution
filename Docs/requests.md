# Demandes inter-équipes — Breizh360

> **Dernière mise à jour :** 2026-01-09  
> **Source :** https://github.com/AnkenBreizh/Breizh360_solution

## Règles
- Une demande = **un livrable**, un **owner**, et une **date cible**.
- Toute demande a un **ID stable** : `XXX-REQ-000`.
- **Done** uniquement si la **remise** est renseignée (chemin/PR/doc).

### Statuts (standard)
- **Backlog** | **Ready** | **In progress** | **Blocked** | **In review** | **Done**

### Priorités (standard)
- **P0** : bloquant critique / prod
- **P1** : bloquant feature en cours
- **P2** : important
- **P3** : nice-to-have

---

## Modèle (copier/coller)
### `XXX-REQ-000` — [TITRE COURT]
- **De :** …
- **À :** …
- **Owner :** … (responsable de suivi côté équipe cible)
- **Priorité :** P0 | P1 | P2 | P3
- **Statut :** Backlog | Ready | In progress | Blocked | In review | Done
- **Nécessaire pour :** `INIT-XXX-000` / Feature (`AUTH`/`USR`/`NOTIF`)
- **Date cible :** …
- **Détails :**
  - …
- **Critères d’acceptation :**
  - …
- **Remise attendue :**
  - `chemin/vers/doc.md` ou PR ou fichier
- **Historique :** (optionnel)
  - 2026-01-09 : création

---

## Demandes — USR (Users)

### `USR-REQ-001` — Contrat Domaine Users (entités + repos)
- **De :** API
- **À :** Domaine
- **Owner :** Domaine
- **Priorité :** P1
- **Statut :** Done
- **Nécessaire pour :** `INIT-USR-001` / `USR`
- **Date cible :** 2026-01-09
- **Détails :**
  - Entités Users (invariants : email unique, normalisation, etc.)
  - Contrats repos (IUserRepository, éventuellement Roles si partagés)
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Domaine/interfaces.md` (sections `IF-USR-…`)
  - Remise = chemins vers fichiers + signatures stables
- **Remise :**
  - `/Breizh360.Domaine/interfaces.md` (IF-USR-001)
  - `/Breizh360.Domaine/Common/DomainException.cs`
  - `/Breizh360.Domaine/Users/Entities/User.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/UserId.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/Email.cs`
  - `/Breizh360.Domaine/Users/ValueObjects/DisplayName.cs`
  - `/Breizh360.Domaine/Users/Repositories/IUserRepository.cs`



### `USR-REQ-002` — Contrat API Users (DTO + erreurs + exemples)
- **De :** UI
- **À :** API
- **Owner :** API
- **Priorité :** P1
- **Statut :** Backlog
- **Nécessaire pour :** `INIT-USR-001` / `USR`
- **Date cible :** À planifier
- **Détails :**
  - DTO request/response, erreurs, exemples de payload
  - Pagination / filtres (si prévu)
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Api/interfaces.md` (sections `IF-API-USR-…`)
  - Exemples utilisables côté UI
- **Remise attendue :**
  - `/Breizh360.Api/interfaces.md` (IF-API-USR-…)

---

## Demandes — NOTIF (Notifications)

### `NOTIF-REQ-001` — Contrat hubs/événements (noms + payloads)
- **De :** UI
- **À :** API
- **Owner :** API
- **Priorité :** P1
- **Statut :** Backlog
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** À planifier
- **Détails :**
  - Noms hubs, méthodes, événements poussés, schémas payload
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Api/interfaces.md` (IF-API-NOTIF-…)
  - UI peut implémenter l’abonnement sans ambiguïté
- **Remise attendue :**
  - `/Breizh360.Api/interfaces.md` (IF-API-NOTIF-…)

### `NOTIF-REQ-002` — Proxy Gateway `/hubs/*` + WebSockets (validation)
- **De :** API
- **À :** Passerelle
- **Owner :** Passerelle
- **Priorité :** P1
- **Statut :** Backlog
- **Nécessaire pour :** `INIT-NOTIF-001` / `NOTIF`
- **Date cible :** À planifier
- **Détails :**
  - Routage `/hubs/*`, WebSockets, corrélation `X-Correlation-ID`
- **Critères d’acceptation :**
  - Contrat publié dans `Breizh360.Gateway/interfaces.md` (IF-GATE-NOTIF-…)
  - Validation smoke test (connexion hub via gateway)
- **Remise attendue :**
  - `/Breizh360.Gateway/interfaces.md` (IF-GATE-NOTIF-…)
