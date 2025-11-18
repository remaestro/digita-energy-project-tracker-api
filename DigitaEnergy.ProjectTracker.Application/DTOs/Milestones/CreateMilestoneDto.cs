namespace DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;

public class CreateMilestoneDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public List<int> LinkedTaskIds { get; set; }
}
