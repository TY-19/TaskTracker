namespace TaskTracker.Application.Models;

public class WorkflowStagePostModel : IValidatableModel
{
    public string Name { get; set; } = string.Empty;
}
