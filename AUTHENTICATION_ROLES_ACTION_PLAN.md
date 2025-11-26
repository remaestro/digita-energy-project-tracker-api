# Plan d'Action : Authentification et Gestion des Rôles

## 📊 État des Lieux

### 1. Backend (.NET)

#### Existant ✅
- **Entité User** : `/Domain/Entities/User.cs`
  - Propriétés : Id, Email, FirstName, LastName, Role, AssignedWorkstreams, CreatedAt, UpdatedAt
  - Rôles définis dans enum `UserRole` : PROJECT_MANAGER, STREAM_LEAD, TEAM_MEMBER

- **Base de données**
  - Table Users existe dans les migrations
  - Champ `AssignedWorkstreams` de type string (liste JSON)

#### Manquant ❌
- ❌ **Aucun contrôleur d'authentification** (`AuthController`)
- ❌ Pas de service d'authentification JWT
- ❌ Pas de hashage de mots de passe
- ❌ Pas de middleware d'authentification configuré dans `Program.cs`
- ❌ Pas d'attributs `[Authorize]` sur les contrôleurs existants
- ❌ Pas de filtrage par workstream dans les services
- ❌ Pas de repository ou service User

### 2. Frontend (Angular)

#### Existant ✅
- **Service d'authentification complet** : `auth.service.ts`
  - Login/Logout fonctionnels (mode développement avec utilisateurs de test)
  - Gestion des permissions par rôle
  - 3 utilisateurs de test configurés :
    - `admin@digita-energy.com` (PROJECT_MANAGER)
    - `manager@digita-energy.com` (STREAM_LEAD - lots : Énergie Renouvelable, Stockage)
    - `user@digita-energy.com` (TEAM_MEMBER)

- **Guards**
  - `AuthGuard` : Protection des routes authentifiées
  - `RoleGuard` : Protection par rôle

- **Modèles**
  - `User`, `UserRole`, `UserPermissions` définis dans `models/index.ts`

#### Terminologie "Lot" vs "Stream" 📝
- Interface utilisée : **"Lot"** (français)
- Code/API : **"workstream"** (anglais)
- Cohérence à maintenir

### 3. Analyse des Workstreams

**Workstreams actuels identifiés** :
- Énergie Renouvelable
- Stockage
- Distribution
- Smart Grid

**Utilisation** :
- Chaque Task, Risk et Milestone a un champ `workstream`
- Les Stream Leads ont une liste `assignedWorkstreams`

---

## 🎯 Exigences Fonctionnelles

### Rôle : PROJECT MANAGER
- ✅ Accès complet à tout
- ✅ Modification de toutes les tâches
- ✅ Accès à tous les lots
- ✅ Gestion des utilisateurs
- ✅ Création de rapports

### Rôle : STREAM LEAD (Responsable de Lot)
- ✅ Accès en lecture à tout
- ✅ **Modification uniquement des tâches de ses lots assignés**
- ✅ **Ajout de risques** (tous les lots ou uniquement les siens ?)
- ✅ Dans le Gantt : modification uniquement des tâches de ses lots
- ✅ Création de rapports sur ses lots
- ❌ Pas d'accès à l'administration
- ❌ Pas de gestion des utilisateurs

### Rôle : TEAM MEMBER
- ✅ Accès en lecture uniquement
- ❌ Pas de modification
- ❌ Pas de création
- ❌ Pas d'accès administration

---

## 📋 Plan d'Action Détaillé

### Phase 1 : Backend - Authentification JWT (Priorité : HAUTE) 🔴

#### Tâche 1.1 : Configuration JWT et Dépendances
**Fichiers à créer/modifier** :
- `appsettings.json` - Ajouter configuration JWT
- `DigitaEnergy.ProjectTracker.Api.csproj` - Ajouter packages NuGet

**Packages nécessaires** :
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

**Configuration JWT** :
```json
"JwtSettings": {
  "SecretKey": "your-super-secret-key-minimum-32-characters-long",
  "Issuer": "DigitaEnergyProjectTracker",
  "Audience": "DigitaEnergyProjectTrackerClient",
  "ExpirationHours": 8
}
```

#### Tâche 1.2 : Créer les DTOs d'authentification
**Fichiers à créer** :
- `/Application/DTOs/Auth/LoginRequestDto.cs`
- `/Application/DTOs/Auth/LoginResponseDto.cs`
- `/Application/DTOs/Auth/RegisterRequestDto.cs`
- `/Application/DTOs/Auth/UserDto.cs`
- `/Application/DTOs/Auth/ChangePasswordDto.cs`
- `/Application/DTOs/Auth/ResetPasswordDto.cs`

#### Tâche 1.3 : Repository et Service User
**Fichiers à créer** :
- `/Domain/Interfaces/IUserRepository.cs`
- `/Infrastructure/Repositories/UserRepository.cs`
- `/Application/Interfaces/IAuthService.cs`
- `/Application/Services/AuthService.cs`
- `/Application/Interfaces/IJwtService.cs`
- `/Application/Services/JwtService.cs`

**Fonctionnalités clés** :
- Hashage de mot de passe avec BCrypt
- Génération de tokens JWT
- Validation des credentials
- Refresh token (optionnel pour v1)

#### Tâche 1.4 : Contrôleur d'authentification
**Fichier à créer** :
- `/Api/Controllers/AuthController.cs`

**Endpoints à implémenter** :
```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/logout
POST /api/auth/refresh-token (optionnel)
POST /api/auth/forgot-password
POST /api/auth/reset-password
PUT  /api/auth/change-password
GET  /api/auth/profile
PUT  /api/auth/profile
```

#### Tâche 1.5 : Configuration du middleware
**Fichier à modifier** :
- `/Api/Program.cs`

**Modifications** :
```csharp
// Ajouter JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Configuration JWT
    });

// Dans le pipeline
app.UseAuthentication();
app.UseAuthorization();
```

#### Tâche 1.6 : Seeds de données utilisateurs
**Fichier à créer** :
- `/Infrastructure/Persistence/UserDataSeeder.cs`

**Utilisateurs de test à créer** :
```
1. admin@digita-energy.com (PROJECT_MANAGER)
2. manager1@digita-energy.com (STREAM_LEAD - Énergie Renouvelable, Stockage)
3. manager2@digita-energy.com (STREAM_LEAD - Distribution, Smart Grid)
4. user@digita-energy.com (TEAM_MEMBER)
```

---

### Phase 2 : Backend - Autorisation et Filtrage (Priorité : HAUTE) 🔴

#### Tâche 2.1 : Créer des attributs d'autorisation personnalisés
**Fichiers à créer** :
- `/Api/Authorization/RequireRoleAttribute.cs`
- `/Api/Authorization/RequireWorkstreamAccessAttribute.cs`

#### Tâche 2.2 : Sécuriser les contrôleurs existants
**Fichiers à modifier** :
- `/Api/Controllers/TasksController.cs`
- `/Api/Controllers/RisksController.cs`
- `/Api/Controllers/MilestonesController.cs`

**Exemple pour TasksController** :
```csharp
[Authorize] // Tous les endpoints nécessitent authentification
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    [HttpGet] // Accessible par tous les rôles
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
        [FromQuery] string? workstream = null)
    {
        var user = GetCurrentUser();
        // Filtrer par workstream si STREAM_LEAD
    }

    [HttpPut("{id}")]
    [RequireWorkstreamAccess] // Vérifier accès au workstream de la tâche
    public async Task<IActionResult> UpdateTask(int id, TaskDto taskDto)
    {
        // Vérifier que STREAM_LEAD modifie uniquement ses lots
    }
}
```

#### Tâche 2.3 : Modifier les services pour filtrage par workstream
**Fichiers à modifier** :
- `/Application/Services/TaskService.cs`
- `/Application/Services/RiskService.cs`
- `/Application/Services/MilestoneService.cs`

**Méthodes à ajouter** :
```csharp
Task<IEnumerable<TaskDto>> GetTasksByWorkstreamsAsync(List<string> workstreams);
Task<bool> CanUserModifyTaskAsync(int taskId, User user);
```

#### Tâche 2.4 : Gestion des risques par Stream Lead
**Clarification nécessaire** : Un Stream Lead peut-il ajouter des risques :
- Option A : Uniquement pour ses lots assignés (recommandé)
- Option B : Pour tous les lots

**Fichier à modifier** :
- `/Api/Controllers/RisksController.cs`

```csharp
[HttpPost]
[Authorize(Roles = "PROJECT_MANAGER,STREAM_LEAD")]
public async Task<ActionResult<RiskDto>> CreateRisk(RiskDto riskDto)
{
    var user = GetCurrentUser();
    
    if (user.Role == UserRole.STREAM_LEAD)
    {
        // Vérifier que le risque concerne un de ses workstreams
        if (!user.AssignedWorkstreams.Contains(riskDto.Workstream))
        {
            return Forbid();
        }
    }
    
    // Créer le risque
}
```

---

### Phase 3 : Backend - Contrôleur Users (Priorité : MOYENNE) 🟡

#### Tâche 3.1 : Créer le contrôleur de gestion des utilisateurs
**Fichier à créer** :
- `/Api/Controllers/UsersController.cs`

**Endpoints** :
```
GET    /api/users                    [Authorize(Roles = "PROJECT_MANAGER")]
GET    /api/users/{id}               [Authorize(Roles = "PROJECT_MANAGER")]
POST   /api/users                    [Authorize(Roles = "PROJECT_MANAGER")]
PUT    /api/users/{id}               [Authorize(Roles = "PROJECT_MANAGER")]
DELETE /api/users/{id}               [Authorize(Roles = "PROJECT_MANAGER")]
GET    /api/users/stream-leads       [Authorize] // Pour affectation
```

#### Tâche 3.2 : Service de gestion des utilisateurs
**Fichier à créer** :
- `/Application/Interfaces/IUserService.cs`
- `/Application/Services/UserService.cs`

---

### Phase 4 : Frontend - Intégration Backend (Priorité : HAUTE) 🔴

#### Tâche 4.1 : Modifier AuthService pour utiliser le vrai backend
**Fichier à modifier** :
- `/src/app/core/services/auth.service.ts`

**Modifications** :
- Décommenter les appels HTTP vers `/api/auth/*`
- Supprimer/commenter le code de test en dur
- Gérer correctement le stockage du token JWT

#### Tâche 4.2 : Créer l'intercepteur HTTP pour JWT
**Fichier à modifier** :
- `/src/app/core/interceptors/auth.interceptor.ts`

**Fonctionnalités** :
```typescript
intercept(req: HttpRequest<any>, next: HttpHandler) {
  const token = this.authService.getToken();
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  return next.handle(req);
}
```

#### Tâche 4.3 : Gestion des erreurs 401/403
**Fichier à créer/modifier** :
- `/src/app/core/interceptors/error.interceptor.ts`

**Fonctionnalités** :
- Intercepter les erreurs 401 → rediriger vers login
- Intercepter les erreurs 403 → afficher message "Accès refusé"

---

### Phase 5 : Frontend - Restrictions UI par rôle (Priorité : HAUTE) 🔴

#### Tâche 5.1 : Modifier le composant Tasks pour filtrage
**Fichier à modifier** :
- `/src/app/features/tasks/tasks.component.ts`

**Logique** :
```typescript
ngOnInit() {
  const user = this.authService.currentUser;
  
  if (user?.role === UserRole.STREAM_LEAD) {
    // Filtrer uniquement les tâches des workstreams assignés
    this.loadTasksForWorkstreams(user.assignedWorkstreams);
  } else {
    // Charger toutes les tâches
    this.loadAllTasks();
  }
}

canEditTask(task: Task): boolean {
  const permissions = this.authService.getUserPermissions();
  
  if (permissions.canEditAllTasks) {
    return true;
  }
  
  if (permissions.canEditAssignedWorkstreams && 
      permissions.assignedWorkstreams?.includes(task.workstream)) {
    return true;
  }
  
  return false;
}
```

#### Tâche 5.2 : Modifier le composant Risks
**Fichier à modifier** :
- `/src/app/features/risks/risks.component.ts`

**Logique** :
```typescript
canCreateRisk(): boolean {
  const user = this.authService.currentUser;
  return user?.role === UserRole.PROJECT_MANAGER || 
         user?.role === UserRole.STREAM_LEAD;
}

canEditRisk(risk: Risk): boolean {
  const user = this.authService.currentUser;
  
  if (user?.role === UserRole.PROJECT_MANAGER) {
    return true;
  }
  
  if (user?.role === UserRole.STREAM_LEAD) {
    return user.assignedWorkstreams?.includes(risk.workstream) || false;
  }
  
  return false;
}
```

#### Tâche 5.3 : Modifier le composant Gantt/Planning
**Fichier à modifier** :
- `/src/app/features/planning/planning.component.ts`

**Logique** :
```typescript
// Dans la configuration dhtmlx-gantt
gantt.attachEvent("onBeforeTaskUpdate", (id, task) => {
  const user = this.authService.currentUser;
  
  if (user?.role === UserRole.STREAM_LEAD) {
    if (!user.assignedWorkstreams?.includes(task.workstream)) {
      alert("Vous ne pouvez modifier que les tâches de vos lots assignés");
      return false;
    }
  }
  
  return true;
});
```

#### Tâche 5.4 : Désactiver visuellement les boutons/actions interdites
**Fichiers à modifier** :
- Tous les composants avec actions (edit, delete, create)

**Exemple** :
```html
<button 
  [disabled]="!canEditTask(task)"
  [class.opacity-50]="!canEditTask(task)"
  [class.cursor-not-allowed]="!canEditTask(task)">
  Modifier
</button>
```

---

### Phase 6 : Renommage "Workstream" → "Stream" (Priorité : BASSE) 🟢

#### Tâche 6.1 : Frontend - Renommage visuel
**Fichiers à modifier** :
- Tous les templates `.component.ts` utilisant "Lot de travaux"
- Remplacer par "Stream" (si c'est ce que vous voulez en français)

**Note** : Actuellement, le frontend utilise déjà "Lot" en français. Clarification nécessaire :
- Voulez-vous remplacer "Lot" par "Stream" en français ?
- Ou garder "Lot" en français et "workstream" en anglais dans le code ?

**Recommandation** : Garder "Lot" en français dans l'interface, "workstream" dans le code.

---

## 🚀 Ordre d'Implémentation Recommandé

### Sprint 1 (1-2 semaines) - Authentification de base
1. ✅ Phase 1 : Backend JWT complet (Tâches 1.1 à 1.6)
2. ✅ Phase 4 : Frontend intégration backend (Tâches 4.1 à 4.3)
3. ✅ Test end-to-end : Login/Logout fonctionnel

### Sprint 2 (1-2 semaines) - Autorisation et filtrage
4. ✅ Phase 2 : Backend autorisation (Tâches 2.1 à 2.4)
5. ✅ Phase 5 : Frontend restrictions UI (Tâches 5.1 à 5.4)
6. ✅ Tests des permissions par rôle

### Sprint 3 (3-5 jours) - Gestion utilisateurs
7. ✅ Phase 3 : Backend Users controller (Tâches 3.1 à 3.2)
8. ✅ Frontend page d'administration des utilisateurs
9. ✅ Tests complets

### Sprint 4 (optionnel) - Polish
10. ✅ Phase 6 : Renommage si nécessaire
11. ✅ Documentation API
12. ✅ Tests unitaires et d'intégration

---

## 📝 Checklist de Validation

### Scénarios de test à valider

#### PROJECT MANAGER
- [ ] Peut se connecter
- [ ] Voit toutes les tâches de tous les lots
- [ ] Peut modifier n'importe quelle tâche
- [ ] Peut créer/modifier/supprimer des risques
- [ ] Peut créer/modifier/supprimer des jalons
- [ ] Accède à l'administration
- [ ] Peut gérer les utilisateurs

#### STREAM LEAD
- [ ] Peut se connecter
- [ ] Voit toutes les tâches (lecture seule pour les autres lots)
- [ ] Peut modifier uniquement les tâches de ses lots assignés
- [ ] Peut créer des risques pour ses lots
- [ ] Dans le Gantt, peut déplacer uniquement ses tâches
- [ ] Ne peut pas accéder à l'administration
- [ ] Ne peut pas gérer les utilisateurs

#### TEAM MEMBER
- [ ] Peut se connecter
- [ ] Voit toutes les tâches en lecture seule
- [ ] Ne peut rien modifier
- [ ] Ne peut rien créer
- [ ] Ne peut rien supprimer

#### Sécurité
- [ ] Les endpoints API nécessitent authentification
- [ ] Les rôles sont vérifiés côté serveur
- [ ] Les mots de passe sont hashés
- [ ] Les tokens JWT expirent après 8h
- [ ] Impossible de contourner les restrictions côté client

---

## 🔧 Configuration Recommandée

### Variables d'environnement
```bash
# Backend
JWT_SECRET_KEY=your-super-secret-key-minimum-32-characters-long
JWT_ISSUER=DigitaEnergyProjectTracker
JWT_AUDIENCE=DigitaEnergyProjectTrackerClient
JWT_EXPIRATION_HOURS=8

# Frontend
API_URL=http://localhost:5046/api
```

### Base de données - Migration
```bash
dotnet ef migrations add AddUserAuthentication
dotnet ef database update
```

---

## 📚 Ressources et Documentation

### Documentation à créer
1. **API Documentation** : Swagger avec exemples d'authentification
2. **Guide développeur** : Comment ajouter de nouveaux endpoints sécurisés
3. **Guide utilisateur** : Explication des rôles et permissions

### Packages et versions
- .NET 8.0
- Angular 19
- JWT Bearer 8.0.0
- BCrypt.Net-Next 4.0.3

---

## ⚠️ Points d'attention

1. **Sécurité** : 
   - Ne jamais faire confiance aux validations côté client uniquement
   - Toujours valider les permissions côté serveur
   - Utiliser HTTPS en production

2. **Performance** :
   - Mettre en cache les permissions utilisateur
   - Indexer le champ `workstream` en base de données

3. **UX** :
   - Afficher des messages clairs quand une action est interdite
   - Désactiver visuellement les boutons inaccessibles
   - Ne pas cacher complètement les fonctionnalités, juste les désactiver

4. **Clarifications nécessaires** :
   - Stream Lead peut-il ajouter des risques pour TOUS les lots ou uniquement les siens ?
   - Faut-il vraiment renommer "Lot" en "Stream" dans l'interface française ?

---

## 💡 Améliorations Futures (Post-MVP)

1. **Refresh Token** : Éviter la déconnexion automatique
2. **2FA** : Double authentification
3. **Audit Log** : Tracer toutes les modifications
4. **Permissions granulaires** : Au-delà des 3 rôles de base
5. **Notifications** : Alerter les Stream Leads des changements dans leurs lots
6. **Historique** : Voir qui a modifié quoi et quand

---

**Date de création** : 23 novembre 2025  
**Auteur** : GitHub Copilot  
**Version** : 1.0
