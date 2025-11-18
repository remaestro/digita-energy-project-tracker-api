using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IRiskService
{
    Task<IEnumerable<RiskDto>> GetAllRisksAsync();
    Task<RiskDto> GetRiskByIdAsync(int id);
    Task<RiskDto> CreateRiskAsync(CreateRiskDto riskDto);
    Task UpdateRiskAsync(int id, UpdateRiskDto riskDto);
    Task DeleteRiskAsync(int id);
}
