using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class SubpartPostPutModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
    [Required]
    public int AssignmentId { get; set; }
}
