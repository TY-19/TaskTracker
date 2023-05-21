namespace TaskTracker.Application.Models;

public class EmployeePostModel : IValidatableModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
