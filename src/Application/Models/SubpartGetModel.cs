namespace TaskTracker.Application.Models;

public class SubpartGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PercentValue { get; set; }
    public bool IsCompleted { get; set; }
    public int AssignmentId { get; set; }
}
