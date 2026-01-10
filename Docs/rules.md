# Règles de collaboration — Breizh360

> **Dernière mise à jour :** 2026-01-09  
> **Source de vérité :** GitHub (commit obligatoire) — https://github.com/AnkenBreizh/Breizh360_solution

## Sources de vérité (à lire dans cet ordre)
- `README.md` (entrée principale)
- `Docs/initiatives.md` (tableau de bord : quoi / qui / statut / prochaine étape)
- `Docs/requests.md` (demandes inter-équipes : blocages, dépendances, livrables)
- `Docs/interfaces_index.md` (où trouver les contrats)
- `Docs/risks.md` (registre des risques)
- `Docs/decisions/` (ADRs : décisions d’architecture)
- Dans chaque projet : `README.md`, `tasks.md`, `interfaces.md`

## Principes non négociables
1. **ID stable obligatoire** (tâches, interfaces, demandes, initiatives)
2. **Fini = Remise** (chemin vers code/doc/PR). Sans remise → pas fini.
3. **Contrats avant implémentation** (documenter avant de consommer)
4. **Aucune supposition inter-équipe** (tout ce qui est flou devient une demande)
5. **Un blocage = une demande** dans `/Docs/requests.md`



## Convention d’ID pour interfaces (anti-collision)
Pour éviter les collisions entre équipes, un ID d’interface doit être préfixé par le **scope** :

- `IF-DOM-...` : Domaine
- `IF-DATA-...` : Données
- `IF-MET-...` : Métier
- `IF-API-...` : API
- `IF-GATE-...` : Passerelle
- `IF-UI-...` : UI
- `IF-TEST-...` : Tests

Exception tolérée : un contrat purement “transverse” explicitement décidé en ADR (ex : `IF-NOTIF-001` côté Domaine), mais **un seul propriétaire**.


## Workflow standard (statuts)
Utiliser **les mêmes statuts** partout (initiatives, demandes, tâches), pour que le suivi reste lisible.

- **Backlog** : identifié mais pas prêt
- **Ready** : prêt à être pris (contrats/dépendances clarifiés)
- **In progress** : en cours
- **Blocked** : bloqué (doit référencer une demande `XXX-REQ-…`)
- **In review** : en validation (tests, relecture, intégration)
- **Done** : terminé **avec remise** (chemin/PR/doc)

## Accès aux interfaces des autres équipes (RÈGLE OFFICIELLE)
Chaque équipe **peut et doit consulter** les fichiers `interfaces.md` des autres équipes selon ses besoins.
- interface non documentée = **inexistante**
- consommation implicite = **interdite**
- flou/manque/incohérence → **demande** dans `/Docs/requests.md`

## Ordre de réalisation
**Domaine → Données → Métier → API → Gateway → UI**  
Tests transverses.

## Règles de changement (contrats / interfaces)
- Chaque fichier `interfaces.md` maintient un **en-tête** (version, statut, date, responsables).
- Un **breaking change** (signature, DTO, règles) implique :
  1) une **REQ** dédiée (pour aligner les consommateurs),
  2) une note de migration (dans l’interface ou une ADR),
  3) une **bump de version** des contrats.

## Features suivies
- `AUTH` — Authentification/Autorisation
- `USR` — Users
- `NOTIF` — Notifications (SignalR + notifications applicatives)
