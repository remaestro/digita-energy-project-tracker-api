namespace DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;

public class UpdateMilestoneDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
}
