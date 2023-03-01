namespace TaskTracker.Application.Models;

public class AssignmentGetModel
{
    public int Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public int BoardId { get; set; }
    public int StageId { get; set; }
    public int? ResponsibleEmployeeId { get; set; }

    public ICollection<SubpartGetModel> Subparts { get; set; } = new List<SubpartGetModel>();
}
