namespace TaskTracker.Application.Models;

public class SubpartPutModel : IValidatableModel
{
    public string? Name { get; set; }
    public int? PercentValue { get; set; }
    public bool? IsCompleted { get; set; }
}
