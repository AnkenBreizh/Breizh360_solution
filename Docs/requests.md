# Demandes inter-équipes — Breizh360

> **Dernière mise à jour : 08 / 01 / 2026**  
> **Règle :** une demande = un ID stable + un “pourquoi” + une remise attendue.

## Modèle (copier/coller)

### `XXX-REQ-000` — [TITRE]
- **De :** Équipe source
- **À :** Équipe cible
- **Nécessaire pour :** INIT-XXX / Feature / Ticket
- **Détails :**
  - ...
- **Remise attendue :**
  - `chemin/vers/doc.md` ou PR ou fichier
- **Statut :** Demande | En cours | Terminé
- **Dernière mise à jour :** 2026-01-08

---

## Demandes

### Demande
|ID|De|À|Nécessaire pour|Détails / Remise|Statut|
|---|---|---|---|---|---|
|SOC-REQ-001|Solution|Tous|Démarrage solution|Conventions + structure de projets + ports + variables d’environnement. Remise : docs/00_socle_solution.md + solution Breizh360.sln (références validées).|Demande|
|SOC-REQ-003|Solution|API|Observabilité & diagnostics|Swagger (dev), health checks, logs structurés, gestion d’erreurs homogène. Remise : Breizh360.Api.Metier/Program.cs + docs/api/diagnostics.md .|Demande|
|SOC-REQ-004|Solution|Tests|CI (build + tests)|Pipeline CI qui compile la solution et exécute les tests. Remise : fichier de pipeline (ex : .github/workflows/ci.yml ou azure-pipelines.yml ) + note dans docs/ci.md .|Demande|
|AUTH-REQ-002|Passerelle|API|Validation JWT|Issuer/audience, signature, durées, refresh, contrat d’erreurs. Remise : docs/auth/02_contrat_jwt.md|Demande|
|AUTH-REQ-003|UI|API|Écran de login|Endpoints + exemples + swagger + mapping 401/403. Remise : Swagger https://localhost:5101/swagger + docs/auth/03_exemples_api.md|Demande|
|AUTH-REQ-004|Métier|Données|Déblocage intégration Auth (Métier)|Aligner les implémentations de dépôts avec les interfaces du Domaine (notamment IPermissionRepository : GetByIdAsync , GetByCodeAsync , ListAsync ) et fournir une méthode de recherche user par login/email (ou 2 méthodes séparées). Remise : build vert + Breizh360.Data/Auth/Repositories/*.cs corrigés.|Demande|
|AUTH-REQ-005|Métier|Domaine|Implémentation validation identifiants|Confirmer/Documenter la stratégie de hash mot de passe (algo, format du hash, salt, versioning) et l’API attendue (ex : User.VerifyPassword(password) ou service dédié). Remise : doc courte dans Breizh360.Domaine/01_modele_domaine.md (ou fichier dédié) + signature(s) publiques.|Demande|


### En cours
_Aucune._

### Terminé
|ID|De|À|Nécessaire pour|Détails / Remise|Statut|
|---|---|---|---|---|---|
|SOC-REQ-002|Solution|Passerelle|Proxy centralisé|Reverse proxy Gateway ( /api/* , /hubs/* ), WebSockets, CORS, corrélation. Remise : config committée dans Breizh360.Gateway + doc docs/gateway/reverse_proxy.md .|Terminé|
|AUTH-REQ-001|API|Domaine|INIT-001 — socle Auth (modèle)|Endpoints d’authentification : modèle User / Role / Permission + RefreshToken (rotation) + audit/soft delete + normalisation. Remise : Breizh360.Domaine/01_modele_domaine.md + code Breizh360.Domaine/Auth/* et Breizh360.Domaine/Common/*|Terminé|

