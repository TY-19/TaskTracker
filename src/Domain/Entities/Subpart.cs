using Domain.Entities;

namespace TaskTracker.Domain.Entities;

public class Subpart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
    public int AssignmentId { get; set; }
    public int StageId { get; set; }

    public Assignment? Assignments { get; set; }
    public WorkflowStage? Stage { get; set; }
}
