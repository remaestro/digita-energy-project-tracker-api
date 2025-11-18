using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Application.Features.Tasks;

public class TaskService : ITaskService
{
    private readonly ProjectTrackerDbContext _context;
    private readonly IMapper _mapper;

    public TaskService(ProjectTrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<TaskDto> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(TaskDto taskDto)
    {
        var task = _mapper.Map<Domain.Entities.Task>(taskDto);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return _mapper.Map<TaskDto>(task);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(int id, TaskDto taskDto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            // Handle not found case
            return;
        }

        _mapper.Map(taskDto, task);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
