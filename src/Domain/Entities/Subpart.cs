using Domain.Entities;

namespace TaskTracker.Domain.Entities;

/// <summary>
/// Represents a part of the task
/// </summary>
public class Subpart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
    public int AssignmentId { get; set; }
}
