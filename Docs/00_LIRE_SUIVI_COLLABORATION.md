# Breizh360 — Prompts par groupe + usage de suivi_collaboration

Ces fichiers sont des prompts à copier-coller pour cadrer le travail par équipe, en respectant les contraintes et décisions d’architecture.

## Fichier de suivi (source de vérité opérationnelle)
- Ouvrir : `suivi_collaboration_fr_v4_tests_fixes.html` (ou le fichier équivalent dans votre repo).
- IMPORTANT : sans JavaScript, cocher une case dans le navigateur **ne sauvegarde pas**.
  - Pour persister : modifier le HTML (ajouter `checked` et ajuster le badge), puis commit Git.

## Comment “lire où on en est”
1) Ouvrir l’onglet **Vue générale** → lire les initiatives et le statut global.
2) Ouvrir l’onglet de **votre équipe** → repérer les tâches avec statut :
   - **À faire** : non démarré
   - **En cours** : travail en cours
   - **Terminé** : livré + remise renseignée
   - **Demande** : dépend d’un autre groupe
3) Si une tâche est “Demande”, aller dans **Demandes inter-équipes** pour voir :
   - qui doit livrer, ce qui est attendu, et le lien/chemin de remise.

## Comment “faire une demande à un autre groupe”
1) Aller dans l’onglet **Demandes inter-équipes**.
2) Ajouter une ligne avec un ID stable (ex: `AUTH-REQ-010`), et remplir :
   - **De** : votre équipe
   - **À** : équipe ciblée
   - **Nécessaire pour** : votre livrable
   - **Détails / Remise** : ce que vous attendez précisément + format (chemin/PR/doc)
   - **Statut** : `Demande`
3) Optionnel : reporter l’ID de demande dans la tâche de votre équipe.

## Comment clôturer une tâche
- Passer le badge en **Terminé** et renseigner **Remise :** (chemin/PR/doc).
- Ajouter `checked` à la checkbox.
- Commit Git.

---
Conseil : gardez les IDs stables. Si le besoin change, créez une nouvelle demande (nouvel ID).
