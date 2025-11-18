using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
    public List<string>? AssignedWorkstreams { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
