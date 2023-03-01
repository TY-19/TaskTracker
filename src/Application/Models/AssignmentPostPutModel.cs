using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class AssignmentPostPutModel
{
    [Required]
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    [Required]
    public int StageId { get; set; }
    public int? ResponsibleEmployeeId { get; set; }
}
