namespace TaskTracker.Domain.Entities;

/// <summary>
/// Represents an task on the board
/// </summary>
public class Assignment
{
    public int Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public int BoardId { get; set; }
    public int StageId { get; set; }
    public int? ResponsibleEmployeeId { get; set; }

    public Board Board { get; set; } = null!;
    public Employee? ResponsibleEmployee { get; set; }
    public WorkflowStage Stage { get; set; } = null!;
    public ICollection<Subpart> Subparts { get; set; } = new List<Subpart>();
}
