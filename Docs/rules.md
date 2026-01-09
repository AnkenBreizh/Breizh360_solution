# Règles de collaboration — Breizh360

> **Dernière mise à jour :** 2026-01-09  
> **Source de vérité :** GitHub (commit obligatoire) — https://github.com/AnkenBreizh/Breizh360_solution

## Principes non négociables
1. **ID stable obligatoire** (tâches, interfaces, demandes)
2. **Fini = Remise** (chemin vers code/doc/PR). Sans remise → pas fini.
3. **Contrats avant implémentation** (documenter avant de consommer)
4. **Aucune supposition inter-équipe**
5. **Un blocage = une demande** dans `/docs/requests.md`

## Accès aux interfaces des autres équipes (RÈGLE OFFICIELLE)
Chaque équipe **peut et doit consulter** les fichiers `interfaces.md` des autres équipes selon ses besoins.
- interface non documentée = **inexistante**
- consommation implicite = **interdite**
- flou/manque/incohérence → **demande** dans `/docs/requests.md`

## Ordre de réalisation
**Domaine → Données → Métier → API Métier → Gateway → UI**  
Tests transverses.

## Features suivies
- `AUTH` — Authentification/Autorisation
- `USR` — Users
- `NOTIF` — Notifications (SignalR + notifications applicatives)
