using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<TaskDto> GetTaskByIdAsync(int id);
    Task<TaskDto> CreateTaskAsync(TaskDto taskDto);
    System.Threading.Tasks.Task UpdateTaskAsync(int id, TaskDto taskDto);
    System.Threading.Tasks.Task DeleteTaskAsync(int id);
}
