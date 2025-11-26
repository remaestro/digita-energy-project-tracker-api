using System.ComponentModel.DataAnnotations;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Le mot de passe actuel est requis")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string NewPassword { get; set; } = string.Empty;
}
