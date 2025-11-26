using DigitaEnergy.ProjectTracker.Domain.Entities;

namespace DigitaEnergy.ProjectTracker.Domain.Interfaces;

public interface IUserRepository
{
    System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<User?> GetByEmailAsync(string email);
    System.Threading.Tasks.Task<IEnumerable<User>> GetAllAsync();
    System.Threading.Tasks.Task<User> CreateAsync(User user);
    System.Threading.Tasks.Task UpdateAsync(User user);
    System.Threading.Tasks.Task DeleteAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsAsync(string email);
}
