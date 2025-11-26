using System.ComponentModel.DataAnnotations;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "L'adresse e-mail est requise")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    public string Password { get; set; } = string.Empty;
}
