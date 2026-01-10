# Tâches — Interface utilisateur (Breizh360.UI)

> **Dernière mise à jour :** 2026-01-10  
> **Règle :** **Done = Remise** (chemin/PR renseigné)

## Statuts (standard)
Backlog | Ready | In progress | Blocked | In review | Done

---

## AUTH (Authentification)

### `AUTH-UI-001` — Page Login (page de démarrage)
- **Owner :** UI
- **Statut :** **Done**
- **Dépendances :** —
- **Objectif :** fournir une page de connexion propre (form + validation) et faire démarrer l’application sur `/`.
- **Critères d’acceptation :**
  - Routes : `/` et `/login`.
  - Formulaire : email + mot de passe + remember me + validation.
  - Design lisible (responsive) sans dépendance externe.
  - Lien de sortie vers une page d’accueil placeholder.
- **Remise :**
  - `Breizh360.UI/Components/Pages/Login.razor`
  - `Breizh360.UI/Components/Pages/Login.razor.css`
  - `Breizh360.UI/Components/Pages/Home.razor` (route déplacée)

### `AUTH-UI-002` — Branchement auth (API login + stockage token + navigation)
- **Owner :** UI
- **Statut :** **Blocked**
- **Dépendances :** `AUTH-REQ-001` (Contrat API Auth) + `INIT-AUTH-001`
- **Objectif :** authentifier l’utilisateur via API/Gateway, stocker le JWT (et refresh si prévu), puis naviguer vers l’accueil.
- **Critères d’acceptation :**
  - Contrat consommé exclusivement via interface UI (`IF-UI-AUTH-001`).
  - Gestion des erreurs (401/403/429/500) + message utilisateur.
  - Persistance : remember me (à préciser selon décision).
  - Déconnexion.
- **Remise attendue :**
  - `Breizh360.UI/Auth/...` (clients + services + stockage)

---

## USR (Users)

### `USR-UI-001` — Écrans Users (liste/détail)
- **Owner :** UI
- **Statut :** **Blocked** (dépend des contrats amont)
- **Dépendances :** `USR-REQ-002` (Contrat API Users) + `INIT-AUTH-001` (auth/UI)
- **Objectif :** fournir une navigation Users et les écrans *liste* + *détail*.
- **Critères d’acceptation :**
  - Routes UI : `/users` (liste) + `/users/{id}` (détail) + état NotFound.
  - Chargement asynchrone + gestion erreurs (401/403/404/500) avec message utilisateur.
  - Pagination (si prévue au contrat API) ou placeholder explicite si non prévue.
  - Couche d’accès données **via client typé** (voir `IF-UI-USR-001`).
- **Remise attendue :**
  - `Breizh360.UI/Users/...` (pages + services + modèles)

---

## NOTIF (Notifications)

### `NOTIF-UI-001` — Abonnement hub + affichage notifications
- **Owner :** UI
- **Statut :** **Blocked** (contrat hub + proxy gateway à stabiliser)
- **Dépendances :** `NOTIF-REQ-001` (Contrat hub/payload API) + `NOTIF-REQ-002` (Proxy Gateway hubs) + `INIT-AUTH-001` (auth/UI) + `NOTIF-REQ-005` (endpoints inbox)
- **Objectif :** se connecter au hub SignalR via Gateway et afficher les notifications (toast/center).
- **Critères d’acceptation :**
  - Service SignalR isolé derrière un contrat UI (voir `IF-UI-NOTIF-001`).
  - Reconnexion gérée (au minimum : retry simple + état connecté/déconnecté).
  - Affichage : toast + liste (mémoire) avec horodatage.
  - Gestion des erreurs (auth manquante, WS bloqué, payload invalide).
- **Remise attendue :**
  - `Breizh360.UI/Notifications/...` (client hub + UI d’affichage)
