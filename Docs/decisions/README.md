# Décisions d’architecture (ADRs) — Breizh360

Objectif : garder une trace **courte** et **durable** des décisions qui impactent plusieurs équipes.

## Quand créer une ADR ?
- Choix de modèle de données / conventions transverses
- Contrats d’API/hubs (surtout si breaking change)
- Stratégie de migrations, déploiement IIS, observabilité, sécurité
- Décision “optionnelle” à trancher (ex : NOTIF inbox persistée)

## Règles
- Une ADR = 1 fichier Markdown : `ADR-XXXX-titre-court.md`
- Le texte reste bref : 1 page si possible.
- Toute ADR doit mentionner : **Contexte**, **Décision**, **Conséquences**, **Liens**.

## Template
Copier `ADR-0001-template.md`.
