namespace DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;

public class TaskDto
{
    public int Id { get; set; }
    public string? Wbs { get; set; }
    public string? Workstream { get; set; }
    public string? Phase { get; set; }
    public string? Activity { get; set; }
    public string? Asset { get; set; }
    public string? Location { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public int Weight { get; set; }
    public DateTime StartPlanned { get; set; }
    public DateTime EndPlanned { get; set; }
    public DateTime? StartBaseline { get; set; }
    public DateTime? EndBaseline { get; set; }
    public DateTime? StartActual { get; set; }
    public DateTime? EndActual { get; set; }
    public double Progress { get; set; }
    public string? Status { get; set; }
    public string? Responsible { get; set; }
    public string? Dependencies { get; set; }
    public string? Comments { get; set; }
}
