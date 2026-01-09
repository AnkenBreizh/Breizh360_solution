# Onboarding développeur — Breizh360

Repo : https://github.com/AnkenBreizh/Breizh360_solution

## 1) Lire le suivi (source de vérité)
1. `README.md`
2. `Docs/initiatives.md` (quoi / qui / statut)
3. `Docs/rules.md` (règles)
4. `Docs/requests.md` (blocages inter-équipes)
5. `Docs/interfaces_index.md` + `interfaces.md` des équipes

## 2) Ouvrir la solution Visual Studio
- Ouvrir `Breizh360.sln`
- Vérifier la version SDK/.NET ciblée par les projets (.NET 10)

## 3) Démarrer en local (développeur)
- Lancer **API** puis **Gateway** (IIS Express ou Kestrel)
- Accès Swagger : `https://localhost:5101/swagger` (API)
- Gateway : `https://localhost:5001` (proxy `/api/*` et `/hubs/*`)

## 4) Règles clés
- Features : `AUTH`, `USR`, `NOTIF`.
- Tu peux consulter les `interfaces.md` des autres équipes, mais tu ne consommes jamais sans contrat documenté.
- Si tu es bloqué : créer/compléter une **REQ** dans `Docs/requests.md`.
