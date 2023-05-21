namespace TaskTracker.Application.Models;

public class AssignmentPutModel : IValidatableModel
{
    public string? Topic { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public int? StageId { get; set; }
    public int? ResponsibleEmployeeId { get; set; }
}
