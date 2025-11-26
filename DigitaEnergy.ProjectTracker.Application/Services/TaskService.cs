using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ProjectTrackerDbContext _context;

    public TaskService(ProjectTrackerDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks.ToListAsync();
        Console.WriteLine($"[TaskService] Tâches récupérées de la base: {tasks.Count}");
        
        if (tasks.Count > 0)
        {
            var firstTask = tasks.First();
            Console.WriteLine($"[TaskService] Exemple première tâche - Id: {firstTask.Id}, Workstream: '{firstTask.Workstream}', Activity: '{firstTask.Activity}'");
        }

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Wbs = t.Wbs,
            Workstream = t.Workstream,
            Phase = t.Phase,
            Activity = t.Activity,
            Asset = t.Asset,
            Location = t.Location,
            Quantity = t.Quantity,
            Unit = t.Unit,
            Weight = t.Weight,
            StartPlanned = t.StartPlanned,
            EndPlanned = t.EndPlanned,
            StartBaseline = t.StartBaseline,
            EndBaseline = t.EndBaseline,
            StartActual = t.StartActual,
            EndActual = t.EndActual,
            Progress = t.Progress,
            Status = t.Status,
            Responsible = t.Responsible,
            Dependencies = t.Dependencies,
            Comments = t.Comments
        });
    }

    public async System.Threading.Tasks.Task<TaskDto> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            throw new KeyNotFoundException($"Task with ID {id} not found.");

        return new TaskDto
        {
            Id = task.Id,
            Wbs = task.Wbs,
            Workstream = task.Workstream,
            Phase = task.Phase,
            Activity = task.Activity,
            Asset = task.Asset,
            Location = task.Location,
            Quantity = task.Quantity,
            Unit = task.Unit,
            Weight = task.Weight,
            StartPlanned = task.StartPlanned,
            EndPlanned = task.EndPlanned,
            StartBaseline = task.StartBaseline,
            EndBaseline = task.EndBaseline,
            StartActual = task.StartActual,
            EndActual = task.EndActual,
            Progress = task.Progress,
            Status = task.Status,
            Responsible = task.Responsible,
            Dependencies = task.Dependencies,
            Comments = task.Comments
        };
    }

    public async System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(TaskDto taskDto)
    {
        var task = new Domain.Entities.Task
        {
            Wbs = taskDto.Wbs ?? string.Empty,
            Workstream = taskDto.Workstream ?? string.Empty,
            Phase = taskDto.Phase ?? string.Empty,
            Activity = taskDto.Activity ?? string.Empty,
            Asset = taskDto.Asset ?? string.Empty,
            Location = taskDto.Location ?? string.Empty,
            Quantity = taskDto.Quantity,
            Unit = taskDto.Unit ?? string.Empty,
            Weight = taskDto.Weight,
            StartPlanned = taskDto.StartPlanned,
            EndPlanned = taskDto.EndPlanned,
            StartBaseline = taskDto.StartBaseline,
            EndBaseline = taskDto.EndBaseline,
            StartActual = taskDto.StartActual,
            EndActual = taskDto.EndActual,
            Progress = taskDto.Progress,
            Status = taskDto.Status ?? string.Empty,
            Responsible = taskDto.Responsible ?? string.Empty,
            Dependencies = taskDto.Dependencies,
            Comments = taskDto.Comments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        taskDto.Id = task.Id;
        return taskDto;
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(int id, TaskDto taskDto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            throw new KeyNotFoundException($"Task with ID {id} not found.");

        task.Wbs = taskDto.Wbs ?? task.Wbs;
        task.Workstream = taskDto.Workstream ?? task.Workstream;
        task.Phase = taskDto.Phase ?? task.Phase;
        task.Activity = taskDto.Activity ?? task.Activity;
        task.Asset = taskDto.Asset ?? task.Asset;
        task.Location = taskDto.Location ?? task.Location;
        task.Quantity = taskDto.Quantity;
        task.Unit = taskDto.Unit ?? task.Unit;
        task.Weight = taskDto.Weight;
        task.StartPlanned = taskDto.StartPlanned;
        task.EndPlanned = taskDto.EndPlanned;
        task.StartBaseline = taskDto.StartBaseline;
        task.EndBaseline = taskDto.EndBaseline;
        task.StartActual = taskDto.StartActual;
        task.EndActual = taskDto.EndActual;
        task.Progress = taskDto.Progress;
        task.Status = taskDto.Status ?? task.Status;
        task.Responsible = taskDto.Responsible ?? task.Responsible;
        task.Dependencies = taskDto.Dependencies;
        task.Comments = taskDto.Comments;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            throw new KeyNotFoundException($"Task with ID {id} not found.");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}
