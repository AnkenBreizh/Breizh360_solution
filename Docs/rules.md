# Règles de collaboration — Breizh360

> **Dernière mise à jour : 08 / 01 / 2026**  
> **Source de vérité :** ces fichiers Markdown (commit Git obligatoire)

## Charte (ex-onglet “Responsable solution”)

Charte d’utilisation (10 règles) :
Source de vérité :
ce fichier HTML (dans le repo).
Persistance :
une coche/édition dans le navigateur ne suffit pas :
modifier le HTML + commit Git
.
ID stable obligatoire
pour toute tâche et toute demande (ex :
AUTH-API-001
,
ENT-REQ-007
).
“Terminé” = Remise
: chemin fichier, doc, PR, ou rapport. Sans remise → pas terminé.
Une équipe ne lit pas le code d’une autre
: tout passe par l’onglet
Interfaces
du fournisseur.
Le responsable fixe les initiatives
(quoi/objectif).
Chaque équipe décompose
(comment/tâches).
Dépendances explicites
: si bloqué, créer/mettre à jour une ligne dans
Demandes inter-équipes
.
Contrats avant implémentation
: interface/documentation d’abord, code ensuite.
Ordre de réalisation
: Domaine → Données → Métier → API → Passerelle → UI (+ Tests en transversal).
Qualité minimale
: erreurs homogènes, logs corrélés (
X-Correlation-ID
), exemples d’usage à jour.

## Vue générale (ex-onglet “Vue générale”)

Règles :
(voir aussi l’onglet
Responsable solution
pour la charte complète)
1) Chaque tâche a un ID.
2) “Terminé” =
une remise
(chemin fichier, doc, PR).
3) Ordre de réalisation : Domaine → Données → Métier → API → Passerelle → UI (+ Tests).
Note :
l’API Métier est l’autorité pour la validation JWT (la Passerelle ne fait que filtrer/proxifier).
Ports : Gateway 5000/5001, API 5100/5101.

## Initiatives (cockpit)

### Vue générale
|Initiative|Résumé|Dépendances|Statut|
|---|---|---|---|
|INIT-000|Socle solution : conventions, structure de la solution, déploiement IIS (2 sites), Passerelle (reverse proxy), CI (build + tests).|Aucune (pré-requis global)|À faire|
|INIT-001|Authentification & autorisations (JWT + RBAC/ABAC + refresh tokens).|INIT-000 + Domaine → Données → Métier → API → Passerelle → UI|En cours|
|INIT-002|Entités métier principales (à compléter).|INIT-000 + Domaine → Données → Métier → API → Passerelle → UI|À faire|
 

### Responsable solution
|Initiative|Objectif|Responsable(s) attendus|Remise globale|Statut|
|---|---|---|---|---|
|INIT-000|Socle solution : conventions, structure, déploiement IIS (2 sites), observabilité, CI.|Solution + API + Passerelle + Tests|docs/00_socle_solution.md , docs/deploiement/iis.md , pipeline CI|À faire|
|INIT-001|Authentification & autorisations : JWT + refresh + RBAC/ABAC + SignalR proxifié.|Domaine → Données → Métier → API → Passerelle → UI (+ Tests)|docs/auth/* + Swagger + tests|En cours|
|INIT-002|Entités métier principales (ex : Entreprise, etc.) — à lancer via demandes + décomposition par équipes.|Tous (selon entité)|Docs interface + code + tests|À faire|

## Initiative — Auth (exemple complet)

### `INIT-001` — Authentification & autorisations
- **Objectif :** JWT + refresh tokens + RBAC/ABAC + endpoint `/me`.
- **Dépendances :** Domaine → Données → Métier → API Métier → Passerelle → UI (+ Tests)
- **Definition of Done :**
  - API : `/auth/login`, `/auth/refresh`, `/me` documentés + stables (Swagger)
  - Passerelle : proxy `/api/*` vers API + `X-Correlation-ID`
  - UI : login + stockage tokens + refresh + UX 401/403
  - Tests : unitaires Métier + intégration API + proxy Gateway
