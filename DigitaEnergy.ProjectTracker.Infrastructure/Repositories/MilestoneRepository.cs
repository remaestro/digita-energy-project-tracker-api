using DigitaEnergy.ProjectTracker.Domain.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Infrastructure.Repositories;

public class MilestoneRepository : IMilestoneRepository
{
    private readonly ProjectTrackerDbContext _context;

    public MilestoneRepository(ProjectTrackerDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<Milestone?> GetByIdAsync(int id)
    {
        return await _context.Milestones.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<Milestone>> GetAllAsync()
    {
        return await _context.Milestones.ToListAsync();
    }

    public async System.Threading.Tasks.Task AddAsync(Milestone milestone)
    {
        await _context.Milestones.AddAsync(milestone);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task UpdateAsync(Milestone milestone)
    {
        _context.Milestones.Update(milestone);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        var milestone = await _context.Milestones.FindAsync(id);
        if (milestone != null)
        {
            _context.Milestones.Remove(milestone);
            await _context.SaveChangesAsync();
        }
    }
}
