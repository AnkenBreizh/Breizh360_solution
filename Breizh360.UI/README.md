# Interface utilisateur — Breizh360.UI

> **Dernière mise à jour :** 2026-01-10  
> **Owner :** Équipe UI  
> **Stack :** .NET MAUI + BlazorWebView (Razor Components)

## Objectif
Fournir l’application cliente (mobile/desktop) de Breizh360.

- L’UI **consomme** les contrats publiés par Domaine/Métier/API/Gateway.
- L’UI expose des **contrats internes** (clients typés + services) afin d’isoler les pages/composants des détails réseau.

## Sources de vérité (à lire)
- `Docs/rules.md`
- `Docs/requests.md`
- `Docs/interfaces_index.md`
- `Docs/initiatives.md`
- Dans ce projet : `tasks.md`, `interfaces.md`

## Suivi du projet UI
- **Tâches :** `Breizh360.UI/tasks.md`
- **Contrats UI (internes + consommation) :** `Breizh360.UI/interfaces.md`

## Exécuter le projet
- Ouvrir la solution dans Visual Studio.
- Vérifier le **TargetFramework** et les versions de packages dans `Breizh360.UI.csproj`.
- Lancer la cible souhaitée (Android / iOS / Windows / MacCatalyst).

> Note : la configuration de l’accès réseau (base URL Gateway, auth JWT, SignalR) est suivie dans `tasks.md` et documentée dans `interfaces.md`.

## Architecture (conventions)
- **Pages Razor** dans `Components/Pages/`.
- **Page de démarrage :** `Login` (`/` et `/login`).
- **Feature folders** (à créer quand Ready) :
  - `Users/` (écrans + services + modèles)
  - `Notifications/` (SignalR + UI d’affichage)
- **Clients typés** (contrats internes) : `*/Clients/`.
- **Services UI** : `*/Services/`.

## Discipline (rappel)
- **ID stable obligatoire** (tâches, interfaces, demandes).
- **Fini = Remise** (sans chemin/PR : pas Done).
- **Contrats avant implémentation** (interface non documentée = inexistante).
