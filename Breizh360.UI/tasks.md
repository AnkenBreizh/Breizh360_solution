# Tâches — Interface utilisateur (Breizh360.UI)

> **Dernière mise à jour :** 2026-01-09  
> **Règle :** **Done = Remise** (chemin/PR renseigné)

## Statuts (standard)
Backlog | Ready | In progress | Blocked | In review | Done

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
  - Couche d’accès données **via client typé** (voir `IF-USR-001`).
- **Remise attendue :**
  - `Breizh360.UI/Users/...` (pages + services + modèles)

---

## NOTIF (Notifications)

### `NOTIF-UI-001` — Abonnement hub + affichage notifications
- **Owner :** UI
- **Statut :** **Blocked** (contrat hub + proxy gateway à stabiliser)
- **Dépendances :** `NOTIF-REQ-001` (Contrat hub/payload API) + `NOTIF-REQ-002` (Proxy Gateway hubs) + `INIT-AUTH-001` (auth/UI)
- **Objectif :** se connecter au hub SignalR via Gateway et afficher les notifications (toast/center).
- **Critères d’acceptation :**
  - Service SignalR isolé derrière un contrat UI (voir `IF-NOTIF-001`).
  - Reconnexion gérée (au minimum : retry simple + état connecté/déconnecté).
  - Affichage : toast + liste (mémoire) avec horodatage.
  - Gestion des erreurs (auth manquante, WS bloqué, payload invalide).
- **Remise attendue :**
  - `Breizh360.UI/Notifications/...` (client hub + UI d’affichage)
