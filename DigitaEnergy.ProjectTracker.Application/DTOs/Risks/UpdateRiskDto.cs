namespace DigitaEnergy.ProjectTracker.Application.DTOs.Risks;

public class UpdateRiskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Workstream { get; set; }
    public int Probability { get; set; }
    public int Impact { get; set; }
    public string MitigationPlan { get; set; }
    public string Owner { get; set; }
    public string Status { get; set; }
}
