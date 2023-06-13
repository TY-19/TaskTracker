namespace TaskTracker.Application.Models;

public class AssignmentPostModel : IValidatableModel
{
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public int StageId { get; set; }
    public int? ResponsibleEmployeeId { get; set; }
    public bool IsCompleted { get; set; } = false;
}
