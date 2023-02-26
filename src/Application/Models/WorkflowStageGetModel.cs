namespace TaskTracker.Application.Models;

public class WorkflowStageGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int Position { get; set; }
    public int? BoardId { get; set; }
}
