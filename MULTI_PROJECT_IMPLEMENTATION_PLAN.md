# Plan d'Implémentation - Multi-Projets

## Vue d'Ensemble
Transformation de l'application de suivi de projet unique vers une architecture multi-projets avec configuration personnalisable par projet.

## Objectifs
- ✅ Permettre la gestion de plusieurs projets indépendants
- ✅ Configuration personnalisable par projet (phases, statuts, workstreams, couleurs)
- ✅ Isolation des données entre projets
- ✅ Interface utilisateur adaptée pour navigation multi-projets
- ✅ Gestion des permissions par projet

---

## Phase 1 : Modélisation des Données (Backend)

### 1.1 Nouvelle Entité `Project`
**Fichier** : `DigitaEnergy.ProjectTracker.Domain/Entities/Project.cs`

```csharp
- Id (Guid)
- Name (string)
- Description (string)
- Code (string) - Code court unique (ex: "DIG-001")
- StartDate (DateTime)
- EndDate (DateTime?)
- Status (ProjectStatus enum: Active, OnHold, Completed, Archived)
- CreatedAt (DateTime)
- CreatedBy (Guid)
- Color (string) - Couleur principale du projet (hex)
- Logo (string?) - URL ou path du logo
- IsActive (bool)
```

### 1.2 Entité `ProjectConfiguration`
**Fichier** : `DigitaEnergy.ProjectTracker.Domain/Entities/ProjectConfiguration.cs`

```csharp
- Id (Guid)
- ProjectId (Guid) - FK vers Project
- Phases (JSON) - Liste des phases personnalisées
- TaskStatuses (JSON) - Statuts de tâches personnalisés
- MilestoneStatuses (JSON) - Statuts de jalons personnalisés
- Workstreams (JSON) - Workstreams avec couleurs
- CustomFields (JSON) - Champs personnalisés optionnels
```

**Structure JSON exemple** :
```json
{
  "phases": [
    { "id": "phase1", "name": "Initiation", "order": 1, "color": "#3B82F6" },
    { "id": "phase2", "name": "Execution", "order": 2, "color": "#10B981" }
  ],
  "taskStatuses": [
    { "id": "todo", "name": "À faire", "color": "#6B7280" },
    { "id": "in_progress", "name": "En cours", "color": "#3B82F6" },
    { "id": "done", "name": "Terminé", "color": "#10B981" }
  ],
  "workstreams": [
    { "id": "ws1", "name": "Engineering", "color": "#3B82F6", "icon": "engineering" },
    { "id": "ws2", "name": "Commercial", "color": "#10B981", "icon": "business" }
  ]
}
```

### 1.3 Modifications des Entités Existantes

**Task, Milestone, Risk** :
- Ajouter `ProjectId (Guid)` - FK vers Project
- Modifier les relations pour inclure le projet

**User** :
- Relation many-to-many avec Project via `UserProject`

**UserInvitation** :
- Ajouter `ProjectId (Guid)` - Invitation pour un projet spécifique

### 1.4 Nouvelle Entité `UserProject`
**Fichier** : `DigitaEnergy.ProjectTracker.Domain/Entities/UserProject.cs`

```csharp
- Id (Guid)
- UserId (Guid) - FK vers User
- ProjectId (Guid) - FK vers Project
- Role (ProjectRole enum: ProjectManager, StreamLead, TeamMember, Viewer)
- AssignedWorkstreams (string) - JSON des workstreams assignés
- JoinedAt (DateTime)
- IsActive (bool)
```

---

## Phase 2 : Migration de la Base de Données

### 2.1 Scripts de Migration
**Fichier** : `DigitaEnergy.ProjectTracker.Infrastructure/Migrations/`

1. **Créer les nouvelles tables** :
   - `Projects`
   - `ProjectConfigurations`
   - `UserProjects`

2. **Modifier les tables existantes** :
   - Ajouter `ProjectId` à : Tasks, Milestones, Risks, UserInvitations
   - Créer les index sur `ProjectId`
   - Ajouter les contraintes FK

3. **Migration des données existantes** :
   - Créer un projet par défaut "Default Project"
   - Associer toutes les données existantes à ce projet
   - Associer tous les utilisateurs existants à ce projet

### 2.2 Script SQL de Migration
**Fichier** : `migration-to-multi-project.sql`

```sql
-- À créer avec les commandes de migration spécifiques
```

---

## Phase 3 : Couche Application (Backend)

### 3.1 Nouveaux Services

**IProjectService / ProjectService** :
- `CreateProjectAsync(CreateProjectDto)`
- `GetProjectByIdAsync(Guid id)`
- `GetUserProjectsAsync(Guid userId)`
- `UpdateProjectAsync(Guid id, UpdateProjectDto)`
- `ArchiveProjectAsync(Guid id)`
- `GetProjectConfigurationAsync(Guid projectId)`
- `UpdateProjectConfigurationAsync(Guid projectId, ProjectConfigurationDto)`

**IUserProjectService / UserProjectService** :
- `AddUserToProjectAsync(Guid projectId, Guid userId, ProjectRole role)`
- `RemoveUserFromProjectAsync(Guid projectId, Guid userId)`
- `UpdateUserRoleInProjectAsync(Guid projectId, Guid userId, ProjectRole role)`
- `GetProjectMembersAsync(Guid projectId)`
- `GetUserProjectRoleAsync(Guid projectId, Guid userId)`

### 3.2 Modifications des Services Existants

**Tous les services (Task, Milestone, Risk, etc.)** :
- Ajouter validation : l'utilisateur a accès au projet
- Filtrer les données par `ProjectId`
- Ajouter `ProjectId` dans les DTOs

### 3.3 Nouveaux DTOs
**Fichier** : `DigitaEnergy.ProjectTracker.Application/DTOs/Projects/`

- `ProjectDto`
- `CreateProjectDto`
- `UpdateProjectDto`
- `ProjectConfigurationDto`
- `ProjectMemberDto`
- `ProjectSummaryDto`

---

## Phase 4 : API (Controllers)

### 4.1 Nouveau Controller `ProjectsController`
**Fichier** : `DigitaEnergy.ProjectTracker.Api/Controllers/ProjectsController.cs`

**Endpoints** :
```
GET    /api/projects                    - Liste des projets de l'utilisateur
GET    /api/projects/{id}               - Détails d'un projet
POST   /api/projects                    - Créer un projet
PUT    /api/projects/{id}               - Modifier un projet
DELETE /api/projects/{id}               - Archiver un projet
GET    /api/projects/{id}/configuration - Config du projet
PUT    /api/projects/{id}/configuration - Modifier la config
GET    /api/projects/{id}/members       - Membres du projet
POST   /api/projects/{id}/members       - Ajouter un membre
DELETE /api/projects/{id}/members/{userId} - Retirer un membre
GET    /api/projects/{id}/stats         - Statistiques du projet
```

### 4.2 Modifications des Controllers Existants

**Tous les controllers** :
- Ajouter `projectId` comme paramètre ou header
- Valider l'accès au projet avant toute opération
- Filtrer par `ProjectId`

**Exemple** :
```
GET /api/projects/{projectId}/tasks
GET /api/projects/{projectId}/milestones
GET /api/projects/{projectId}/risks
```

---

## Phase 5 : Frontend Angular - Architecture

### 5.1 Nouveau Module de Contexte Projet

**Service** : `ProjectContextService`
**Fichier** : `src/app/core/services/project-context.service.ts`

```typescript
- currentProject$: BehaviorSubject<Project | null>
- userProjects$: Observable<Project[]>
- selectProject(projectId: string): void
- getCurrentProject(): Project | null
- getUserRole(projectId: string): ProjectRole
- hasPermission(projectId: string, permission: string): boolean
```

### 5.2 Nouveaux Modèles
**Fichier** : `src/app/core/models/project.ts`

```typescript
export interface Project {
  id: string;
  name: string;
  code: string;
  description: string;
  color: string;
  logo?: string;
  status: ProjectStatus;
  startDate: Date;
  endDate?: Date;
  configuration: ProjectConfiguration;
}

export interface ProjectConfiguration {
  phases: Phase[];
  taskStatuses: Status[];
  milestoneStatuses: Status[];
  workstreams: Workstream[];
}

export interface Workstream {
  id: string;
  name: string;
  color: string;
  icon: string;
}
```

### 5.3 Nouveau Service API
**Fichier** : `src/app/core/services/project.service.ts`

```typescript
- getProjects(): Observable<Project[]>
- getProject(id: string): Observable<Project>
- createProject(data: CreateProject): Observable<Project>
- updateProject(id: string, data: UpdateProject): Observable<Project>
- getProjectConfiguration(id: string): Observable<ProjectConfiguration>
- updateProjectConfiguration(id: string, config: ProjectConfiguration): Observable<void>
- getProjectMembers(id: string): Observable<ProjectMember[]>
```

---

## Phase 6 : Frontend Angular - Interface Utilisateur

### 6.1 Nouveau Composant : Sélecteur de Projet
**Fichier** : `src/app/shared/components/project-selector/`

**Emplacement** : Header/Navbar
**Fonctionnalités** :
- Dropdown avec liste des projets
- Recherche rapide
- Indicateur visuel du projet actuel (couleur, logo)
- Badge du rôle de l'utilisateur
- Accès rapide aux paramètres du projet

**Design** :
```
┌─────────────────────────────────┐
│ [Logo] Nom du Projet  ▼        │
│ ─────────────────────────────  │
│ Mes Projets:                   │
│ ○ Project Alpha     [Manager]  │
│ ● Project Beta      [Member]   │ ← Projet actuel
│ ○ Project Gamma     [Lead]     │
│ ─────────────────────────────  │
│ ⚙ Gérer les projets            │
│ + Créer un projet              │
└─────────────────────────────────┘
```

### 6.2 Nouvelle Page : Liste des Projets
**Route** : `/projects`
**Fichier** : `src/app/features/projects/project-list/`

**Fonctionnalités** :
- Grille/Liste des projets avec cartes
- Filtres : Actif, Archivé, Tous
- Recherche
- Bouton "Créer un projet" (si Project Manager)
- Statistiques par projet (nb tâches, jalons, progression)

**Design Carte Projet** :
```
┌────────────────────────────────────┐
│ [Logo]  Nom du Projet      [•••]  │
│         Code: PRJ-001              │
│ ──────────────────────────────────│
│ Description courte...              │
│ ──────────────────────────────────│
│ 📊 Progress: ███████░░ 70%        │
│ 📋 45 Tâches  🎯 12 Jalons        │
│ 👥 8 Membres  📅 Jan - Dec 2025   │
│ ──────────────────────────────────│
│ [Ouvrir]            Rôle: Manager │
└────────────────────────────────────┘
```

### 6.3 Nouvelle Page : Détails/Paramètres Projet
**Route** : `/projects/{id}/settings`
**Fichier** : `src/app/features/projects/project-settings/`

**Onglets** :
1. **Général** : Nom, description, dates, logo
2. **Configuration** :
   - Phases personnalisées
   - Statuts de tâches
   - Statuts de jalons
   - Workstreams avec couleurs
3. **Membres** : Gestion des utilisateurs du projet
4. **Permissions** : Configuration des rôles
5. **Danger Zone** : Archiver/Supprimer

### 6.4 Modifications de l'Interface Existante

#### Header/Navbar
- Ajouter le sélecteur de projet (à gauche ou centre)
- Indicateur visuel de couleur du projet (barre en haut)
- Badge du rôle de l'utilisateur

#### Dashboard
- Ajouter titre "Dashboard - [Nom du Projet]"
- Filtrer toutes les stats par projet actuel
- Ajouter bouton rapide "Changer de projet"

#### Toutes les Pages (Tasks, Milestones, Risks, etc.)
- Ajouter breadcrumb : Projet > Section actuelle
- Utiliser les couleurs du projet dans l'UI
- Utiliser les workstreams configurés du projet
- Utiliser les statuts configurés du projet

#### Sidebar/Menu
- Ajouter section "Projet" en haut
- Menu contextuel au projet actuel
- Accès rapide aux paramètres du projet

---

## Phase 7 : Gestion des Permissions

### 7.1 Nouveaux Rôles par Projet
```typescript
enum ProjectRole {
  PROJECT_MANAGER = 'PROJECT_MANAGER',    // Tous droits sur le projet
  STREAM_LEAD = 'STREAM_LEAD',            // Gestion de son workstream
  TEAM_MEMBER = 'TEAM_MEMBER',            // Lecture/Écriture limitée
  VIEWER = 'VIEWER'                        // Lecture seule
}
```

### 7.2 Matrice de Permissions

| Action | Project Manager | Stream Lead | Team Member | Viewer |
|--------|----------------|-------------|-------------|--------|
| Voir les données | ✅ | ✅ | ✅ | ✅ |
| Créer des tâches | ✅ | ✅ (son workstream) | ✅ | ❌ |
| Modifier des tâches | ✅ | ✅ (son workstream) | ✅ (assignées) | ❌ |
| Supprimer des tâches | ✅ | ✅ (son workstream) | ❌ | ❌ |
| Créer des jalons | ✅ | ✅ | ❌ | ❌ |
| Modifier config projet | ✅ | ❌ | ❌ | ❌ |
| Gérer les membres | ✅ | ❌ | ❌ | ❌ |
| Archiver projet | ✅ | ❌ | ❌ | ❌ |

### 7.3 Guards Angular

**ProjectAccessGuard** : Vérifie que l'utilisateur a accès au projet
**ProjectRoleGuard** : Vérifie le rôle de l'utilisateur dans le projet

---

## Phase 8 : Invitations et Onboarding

### 8.1 Modification du Processus d'Invitation

**Nouveau flow** :
1. Project Manager invite un utilisateur **pour un projet spécifique**
2. Email contient le nom du projet
3. Après acceptation, l'utilisateur est ajouté au projet
4. Redirection vers le dashboard du projet

### 8.2 Écran d'Onboarding Premier Projet

Pour les nouveaux utilisateurs sans projet :
- Page d'accueil spéciale
- Options : Créer un projet OU Attendre une invitation
- Guide rapide

---

## Phase 9 : Thématisation Visuelle par Projet

### 9.1 Service de Thème Dynamique
**Fichier** : `src/app/core/services/theme.service.ts`

```typescript
- applyProjectTheme(project: Project): void
- setProjectColor(color: string): void
- resetTheme(): void
```

### 9.2 Variables CSS Dynamiques

Injecter les couleurs du projet dans les variables CSS :
```scss
:root {
  --project-primary: #3B82F6;
  --project-primary-light: #60A5FA;
  --project-primary-dark: #2563EB;
}
```

### 9.3 Éléments Visuels

- **Header** : Barre de couleur du projet
- **Cards** : Bordure de la couleur du projet
- **Boutons primaires** : Couleur du projet
- **Workstream badges** : Couleurs configurées
- **Logo** : Affiché dans le header et sélecteur

---

## Phase 10 : Migration des Données Existantes

### 10.1 Script de Migration

**Objectif** : Migrer les données actuelles vers un "Projet par Défaut"

```sql
-- Créer le projet par défaut
INSERT INTO Projects (Id, Name, Code, Description, Status, CreatedAt, Color)
VALUES (NEWID(), 'Digita Energy Project', 'DIG-001', 'Projet principal', 'Active', GETUTCDATE(), '#3B82F6');

-- Créer la configuration par défaut
INSERT INTO ProjectConfigurations (Id, ProjectId, Phases, TaskStatuses, ...)
VALUES (...);

-- Associer toutes les tâches au projet par défaut
UPDATE Tasks SET ProjectId = (SELECT Id FROM Projects WHERE Code = 'DIG-001');

-- Associer tous les utilisateurs au projet
INSERT INTO UserProjects (UserId, ProjectId, Role, JoinedAt)
SELECT Id, (SELECT Id FROM Projects WHERE Code = 'DIG-001'), 
       CASE 
         WHEN Role = 'PROJECT_MANAGER' THEN 'PROJECT_MANAGER'
         ELSE 'TEAM_MEMBER'
       END, 
       GETUTCDATE()
FROM Users;
```

---

## Phase 11 : Tests

### 11.1 Tests Backend
- Tests unitaires pour les nouveaux services
- Tests d'intégration pour les permissions
- Tests de migration de données

### 11.2 Tests Frontend
- Tests des composants de sélection de projet
- Tests de navigation multi-projets
- Tests des permissions UI

---

## Phase 12 : Documentation

### 12.1 Documentation Technique
- Architecture multi-projets
- Modèle de données
- API endpoints

### 12.2 Documentation Utilisateur
- Guide de création de projet
- Guide de configuration
- Guide de gestion des membres

---

## Ordre d'Implémentation Recommandé

### Sprint 1 (Backend Foundation) - 5-7 jours
1. Créer les entités Project, ProjectConfiguration, UserProject
2. Créer la migration de base de données
3. Implémenter ProjectService et UserProjectService
4. Créer les DTOs
5. Implémenter ProjectsController

### Sprint 2 (Backend Integration) - 5-7 jours
1. Modifier toutes les entités pour ajouter ProjectId
2. Mettre à jour tous les services existants
3. Implémenter la validation des permissions
4. Modifier tous les controllers
5. Tests backend

### Sprint 3 (Frontend Foundation) - 5-7 jours
1. Créer les modèles TypeScript
2. Créer ProjectService et ProjectContextService
3. Créer le sélecteur de projet
4. Créer la page liste des projets
5. Créer les guards

### Sprint 4 (Frontend Integration) - 5-7 jours
1. Modifier le layout pour inclure le contexte projet
2. Mettre à jour toutes les pages existantes
3. Implémenter la thématisation dynamique
4. Créer la page de paramètres projet
5. Tests frontend

### Sprint 5 (Polish & Migration) - 3-5 jours
1. Script de migration des données
2. Documentation
3. Tests end-to-end
4. Corrections de bugs
5. Déploiement

---

## Estimation Totale : 23-31 jours

---

## Risques et Considérations

### Risques Techniques
- **Performance** : Filtrage par projet sur toutes les requêtes
- **Migration** : Perte de données potentielle
- **Compatibilité** : Rétrocompatibilité pendant la transition

### Risques Métier
- **Adoption** : Les utilisateurs devront s'adapter au nouveau modèle
- **Formation** : Besoin de documenter les nouvelles fonctionnalités

### Mitigation
- Tests exhaustifs avant migration
- Backup complet de la base de données
- Déploiement progressif avec feature flags
- Documentation et formation utilisateur

---

## Points de Décision

### À Décider Avant de Commencer :

1. **Les utilisateurs peuvent-ils être dans plusieurs projets ?** → OUI (recommandé)
2. **Un super-admin peut voir tous les projets ?** → À définir
3. **Configuration partagée entre projets ou totalement indépendante ?** → Indépendante
4. **Archivage vs Suppression de projets ?** → Archivage (soft delete)
5. **Limite du nombre de projets par organisation ?** → À définir selon le modèle économique

---

## Prochaines Étapes Immédiates

1. ✅ Valider ce plan d'action
2. ✅ Confirmer les décisions clés
3. ✅ Créer une branche Git : `feature/multi-project`
4. ✅ Commencer Sprint 1 : Backend Foundation

---

**Date de création** : 27 novembre 2025  
**Version** : 1.0  
**Statut** : En attente de validation
