using DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(RegisterRequestDto request);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto request);
    Task DeleteUserAsync(Guid id);
    Task<IEnumerable<UserDto>> GetStreamLeadsAsync();
    Task<bool> UserExistsAsync(Guid id);
}
