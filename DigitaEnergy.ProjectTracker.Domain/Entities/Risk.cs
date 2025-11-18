namespace DigitaEnergy.ProjectTracker.Domain.Entities;

public class Risk
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Workstream { get; set; }
    public int Probability { get; set; } // Values from 1 to 5
    public int Impact { get; set; }      // Values from 1 to 5
    public int Criticality { get; set; }
    public string MitigationPlan { get; set; }
    public string Owner { get; set; }
    public string Status { get; set; } // e.g., "Identifié", "En cours d'atténuation", "Atténué", "Clôturé"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
