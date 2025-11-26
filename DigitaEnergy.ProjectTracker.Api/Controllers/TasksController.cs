using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IWorkstreamAuthorizationService _authorizationService;

    public TasksController(ITaskService taskService, IWorkstreamAuthorizationService authorizationService)
    {
        _taskService = taskService;
        _authorizationService = authorizationService;
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] string? viewMode = null)
    {
        var userId = GetCurrentUserId();
        
        // Si viewMode=gantt, utiliser GetAccessibleWorkstreamsAsync (vue complète)
        // Sinon, utiliser GetFilteredWorkstreamsAsync (vue filtrée par workstreams assignés)
        var accessibleWorkstreams = viewMode?.ToLower() == "gantt"
            ? await _authorizationService.GetAccessibleWorkstreamsAsync(userId)
            : await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        
        Console.WriteLine($"[TasksController] UserId: {userId}, ViewMode: {viewMode}");
        Console.WriteLine($"[TasksController] Workstreams accessibles: {string.Join(", ", accessibleWorkstreams.Select(w => w.ToString()))}");
        
        // Convertir les workstreams enum en valeurs de base de données
        var workstreamDbValues = accessibleWorkstreams.Select(w => w.ToDbValue()).ToList();
        Console.WriteLine($"[TasksController] Workstreams DB values: {string.Join(", ", workstreamDbValues)}");
        
        var allTasks = await _taskService.GetAllTasksAsync();
        Console.WriteLine($"[TasksController] Total tâches en base: {allTasks.Count()}");
        
        // Filtrer les tâches selon les workstreams accessibles
        // Utiliser FromDbValue pour normaliser la comparaison et gérer les variantes
        var filteredTasks = allTasks.Where(t => 
        {
            if (string.IsNullOrWhiteSpace(t.Workstream)) return true;
            
            // Essayer de convertir le workstream de la tâche en enum
            var taskWorkstream = WorkstreamExtensions.FromDbValue(t.Workstream);
            if (!taskWorkstream.HasValue)
            {
                Console.WriteLine($"[TasksController] Workstream non reconnu pour tâche {t.Id}: '{t.Workstream}'");
                return false;
            }
            
            // Vérifier si ce workstream est dans la liste accessible
            var isAccessible = accessibleWorkstreams.Contains(taskWorkstream.Value);
            if (!isAccessible)
            {
                Console.WriteLine($"[TasksController] Tâche {t.Id} avec workstream '{t.Workstream}' ({taskWorkstream.Value}) non accessible");
            }
            return isAccessible;
        }).ToList();
        
        Console.WriteLine($"[TasksController] Tâches filtrées retournées: {filteredTasks.Count}");
        
        var response = new
        {
            Data = filteredTasks,
            TotalCount = filteredTasks.Count,
            Page = 1,
            PageSize = filteredTasks.Count,
            TotalPages = 1
        };
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        
        // Vérifier l'accès au workstream
        var userId = GetCurrentUserId();
        var accessibleWorkstreams = await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        var workstreamDbValues = accessibleWorkstreams.Select(w => w.ToDbValue()).ToList();
        
        if (!string.IsNullOrWhiteSpace(task.Workstream) && !workstreamDbValues.Contains(task.Workstream))
        {
            return Forbid();
        }
        
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(TaskDto taskDto)
    {
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(taskDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(taskDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
        var createdTask = await _taskService.CreateTaskAsync(taskDto);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskDto taskDto)
    {
        if (id != taskDto.Id)
        {
            return BadRequest();
        }
        
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(taskDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(taskDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }

        await _taskService.UpdateTaskAsync(id, taskDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = GetCurrentUserId();
        var task = await _taskService.GetTaskByIdAsync(id);
        
        if (task == null)
        {
            return NotFound();
        }
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(task.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(task.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}
