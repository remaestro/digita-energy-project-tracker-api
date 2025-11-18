using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IMilestoneService
{
    Task<IEnumerable<MilestoneDto>> GetAllMilestonesAsync();
    Task<MilestoneDto> GetMilestoneByIdAsync(int id);
    Task<MilestoneDto> CreateMilestoneAsync(CreateMilestoneDto milestoneDto);
    Task UpdateMilestoneAsync(int id, UpdateMilestoneDto milestoneDto);
    Task DeleteMilestoneAsync(int id);
}
