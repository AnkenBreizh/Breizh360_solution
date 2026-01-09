# Registre des risques — Breizh360

> **Dernière mise à jour :** 2026-01-09

## Règles
- Un risque = une phrase claire + impact + plan.
- Lier si possible à une initiative (`INIT-…`) et/ou une demande (`XXX-REQ-…`).

## Registre

| ID | Risque | Probabilité | Impact | Détection | Mitigation / Plan | Owner | Liens | Statut |
|---:|---|:---:|:---:|---|---|---|---|---|
| RISK-001 | (ex) Contrats d’interfaces instables → rework UI/Gateway | M | H | Breaking changes répétés | Versionner + REQ obligatoire + note migration | Responsable Solution | INIT-* / REQ-* | Open |
| RISK-002 | (ex) WebSockets/IIS (SignalR) mal configuré en intranet | M | H | Impossible de se connecter au hub | Checklist IIS + smoke tests Gateway→API | Passerelle | INIT-NOTIF-001 | Open |
