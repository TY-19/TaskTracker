namespace TaskTracker.Application.Models;

public class BoardPostModel : IValidatableModel
{
    public string Name { get; set; } = string.Empty;
}
