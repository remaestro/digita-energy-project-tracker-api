using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Auth;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using DigitaEnergy.ProjectTracker.Domain.Interfaces;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async System.Threading.Tasks.Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async System.Threading.Tasks.Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async System.Threading.Tasks.Task<UserDto> CreateUserAsync(RegisterRequestDto request)
    {
        // Vérifier si l'email existe déjà
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Un utilisateur avec l'email {request.Email} existe déjà.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            AssignedWorkstreams = request.AssignedWorkstreams != null && request.AssignedWorkstreams.Any() 
                ? string.Join(",", request.AssignedWorkstreams) 
                : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async System.Threading.Tasks.Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Utilisateur avec l'ID {id} introuvable.");
        }

        // Vérifier si l'email est déjà utilisé par un autre utilisateur
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null && existingUser.Id != id)
        {
            throw new InvalidOperationException($"L'email {request.Email} est déjà utilisé par un autre utilisateur.");
        }

        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Role = request.Role;
        user.AssignedWorkstreams = request.AssignedWorkstreams != null && request.AssignedWorkstreams.Any() 
            ? string.Join(",", request.AssignedWorkstreams) 
            : null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async System.Threading.Tasks.Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Utilisateur avec l'ID {id} introuvable.");
        }

        await _userRepository.DeleteAsync(id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<UserDto>> GetStreamLeadsAsync()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var streamLeads = allUsers.Where(u => u.Role == UserRole.STREAM_LEAD);
        return _mapper.Map<IEnumerable<UserDto>>(streamLeads);
    }

    public async System.Threading.Tasks.Task<bool> UserExistsAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null;
    }
}
