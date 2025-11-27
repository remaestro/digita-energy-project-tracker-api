using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Auth;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IMapper mapper,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _mapper = mapper;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        Console.WriteLine($"[AUTH] Login attempt for email: {request.Email}");
        
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            Console.WriteLine($"[AUTH] User not found: {request.Email}");
            return null; // User not found
        }

        Console.WriteLine($"[AUTH] User found: {user.Email}, Role: {user.Role}");
        Console.WriteLine($"[AUTH] Password hash in DB: {user.PasswordHash?.Substring(0, 20)}...");
        Console.WriteLine($"[AUTH] Password provided: {request.Password}");

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            Console.WriteLine($"[AUTH] Password verification FAILED for {request.Email}");
            return null; // Invalid password
        }

        Console.WriteLine($"[AUTH] Password verification SUCCESS for {request.Email}");

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "8");

        return new LoginResponseDto
        {
            Token = token,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };
    }

    public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(request.Email))
        {
            return null; // User already exists
        }

        // Create new user
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
                : null
        };

        await _userRepository.CreateAsync(user);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "8");

        return new LoginResponseDto
        {
            Token = token,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };
    }

    public async Task<UserDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return false; // Current password is incorrect
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            // Return true even if user not found for security reasons
            return true;
        }

        // Generate reset token
        var resetToken = Guid.NewGuid().ToString();
        user.ResetToken = BCrypt.Net.BCrypt.HashPassword(resetToken);
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

        await _userRepository.UpdateAsync(user);

        // Envoyer l'email de réinitialisation
        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH] Failed to send password reset email: {ex.Message}");
        }

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto request)
    {
        // Find user by reset token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => 
            u.ResetToken != null && 
            u.ResetTokenExpiry.HasValue && 
            u.ResetTokenExpiry.Value > DateTime.UtcNow &&
            BCrypt.Net.BCrypt.Verify(request.Token, u.ResetToken));

        if (user == null)
        {
            return false; // Invalid or expired token
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _userRepository.UpdateAsync(user);

        return true;
    }
}
