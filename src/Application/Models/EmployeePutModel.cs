namespace TaskTracker.Application.Models;

public class EmployeePutModel : IValidatableModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
