namespace TaskTracker.Application.Models;

public class SubpartPostModel : IValidatableModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
    public int? AssignmentId { get; set; }
}
