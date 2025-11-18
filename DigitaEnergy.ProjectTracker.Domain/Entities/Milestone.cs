namespace DigitaEnergy.ProjectTracker.Domain.Entities;

public class Milestone
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public List<int> LinkedTaskIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
