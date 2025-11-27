using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Application.DTOs.Invitations;

public class CreateInvitationDto
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
    public List<Workstream> AssignedWorkstreams { get; set; } = new();
}
