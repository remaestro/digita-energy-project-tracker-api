using System.ComponentModel.DataAnnotations;
using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class UpdateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public List<string>? AssignedWorkstreams { get; set; }
}
