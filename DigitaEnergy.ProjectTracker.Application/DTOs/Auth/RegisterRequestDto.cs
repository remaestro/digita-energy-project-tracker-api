using System.ComponentModel.DataAnnotations;
using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "L'adresse e-mail est requise")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est requis")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le rôle est requis")]
    public UserRole Role { get; set; }

    public List<string>? AssignedWorkstreams { get; set; }
}
