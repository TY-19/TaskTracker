namespace TaskTracker.Application.Models;

public class BoardPutModel : IValidatableModel
{
    public string? Name { get; set; }
}
