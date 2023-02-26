namespace TaskTracker.Application.Models;

public class SubpartGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
    public int AssignmentId { get; set; }
    public int StageId { get; set; }
}
