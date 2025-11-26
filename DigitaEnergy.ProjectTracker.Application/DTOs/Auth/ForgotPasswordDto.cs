using System.ComponentModel.DataAnnotations;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Auth;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "L'adresse e-mail est requise")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide")]
    public string Email { get; set; } = string.Empty;
}
