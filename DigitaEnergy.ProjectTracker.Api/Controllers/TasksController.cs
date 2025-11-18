using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        var response = new
        {
            Data = tasks,
            TotalCount = tasks.Count(),
            Page = 1,
            PageSize = tasks.Count(),
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
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(TaskDto taskDto)
    {
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

        await _taskService.UpdateTaskAsync(id, taskDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}
