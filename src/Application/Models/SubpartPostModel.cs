namespace TaskTracker.Application.Models;

public class SubpartPostModel : IValidatableModel
{
    public string Name { get; set; } = string.Empty;
    public int PercentValue { get; set; }
    public bool IsCompleted { get; set; }
    public int AssignmentId { get; set; }
}
