namespace TaskTracker.Domain.Entities;

/// <summary>
/// Represents a stage of the workflow through which every task on the board goes.
/// </summary>
public class WorkflowStage
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int Position { get; set; }
    public int BoardId { get; set; }
    public Board Board { get; set; } = new Board();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
