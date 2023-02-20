using Domain.Entities;

namespace TaskTracker.Domain.Entities;

public class WorkflowStage
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int Position { get; set; }
    public int? BoardId { get; set; }
    public Board? Board { get; set; }
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Subpart> Subparts { get; set; } = new List<Subpart>();
}
