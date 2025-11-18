using DigitaEnergy.ProjectTracker.Domain.Entities;

namespace DigitaEnergy.ProjectTracker.Domain.Interfaces;

public interface IMilestoneRepository
{
    System.Threading.Tasks.Task<Milestone?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<Milestone>> GetAllAsync();
    System.Threading.Tasks.Task AddAsync(Milestone milestone);
    System.Threading.Tasks.Task UpdateAsync(Milestone milestone);
    System.Threading.Tasks.Task DeleteAsync(int id);
}
