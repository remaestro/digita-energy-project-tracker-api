using DigitaEnergy.ProjectTracker.Domain.Entities;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}
