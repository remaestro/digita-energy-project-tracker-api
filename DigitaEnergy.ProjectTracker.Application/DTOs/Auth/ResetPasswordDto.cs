using System.ComponentModel.DataAnnotations;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Le token est requis")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string NewPassword { get; set; } = string.Empty;
}
