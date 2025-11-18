namespace DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;

public class CreateMilestoneDto
{
    public string? Code { get; set; }
    public string Title { get; set; }
    public string? Workstream { get; set; }
    public DateTime DatePlanned { get; set; }
    public DateTime? DateActual { get; set; }
    public string Status { get; set; }
    public string? Comments { get; set; }
    public List<int>? LinkedTaskIds { get; set; }
}
