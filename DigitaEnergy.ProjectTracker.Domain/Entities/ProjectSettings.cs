namespace DigitaEnergy.ProjectTracker.Domain.Entities;

public class ProjectSettings
{
    public Guid Id { get; set; }
    public DateTime T0Date { get; set; }
    public int LowThreshold { get; set; }
    public int MediumThreshold { get; set; }
    public int HighThreshold { get; set; }
    public int CriticalThreshold { get; set; }
    public int MilestoneAlertDays { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
