using Domain.Entities;

namespace TaskTracker.Domain.Entities;

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
