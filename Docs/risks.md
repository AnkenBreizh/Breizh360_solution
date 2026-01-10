# Registre des risques — Breizh360

> **Dernière mise à jour :** 2026-01-10

## Règles
- Un risque = une phrase claire + impact + plan.
- Lier si possible à une initiative (`INIT-…`) et/ou une demande (`XXX-REQ-…`).

## Registre

| ID | Risque | Probabilité | Impact | Détection | Mitigation / Plan | Owner | Liens | Statut |
|---:|---|:---:|:---:|---|---|---|---|---|
| RISK-001 | (ex) Contrats d’interfaces instables → rework UI/Gateway | M | H | Breaking changes répétés | Versionner + REQ obligatoire + note migration | Responsable Solution | INIT-* / REQ-* | Open |
| RISK-002 | WebSockets/IIS (SignalR) mal configuré en intranet | M | H | Connexion hub impossible / instable | **Gateway proxy hubs implémenté** + smoke tests doc (`Breizh360.Gateway/README.md`). Reste : checklist IIS (WebSockets, headers, timeouts) + validation end-to-end en environnement cible. | Passerelle | INIT-NOTIF-001, NOTIF-REQ-002 | Open |
| RISK-003 | Stratégie de migrations / application en CI/CD non cadrée → dérive schéma / incidents de déploiement | M | H | Scripts manuels, migrations non appliquées, décalage entre environnements | Standardiser : conventions de nommage, procédure `dotnet ef migrations script`, étape CI pour générer/appliquer (ou runbook), vérif de compatibilité avant release | Données | INIT-NOTIF-001, NOTIF-REQ-004 | Open |
