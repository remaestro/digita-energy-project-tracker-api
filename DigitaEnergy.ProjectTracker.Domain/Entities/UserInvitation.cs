using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Domain.Entities;

public class UserInvitation
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid InvitedByUserId { get; set; }
    public UserRole Role { get; set; }
    public List<Workstream> AssignedWorkstreams { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }

    // Navigation properties
    public User? InvitedBy { get; set; }
}
