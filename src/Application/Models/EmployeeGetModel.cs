namespace TaskTracker.Application.Models;

public class EmployeeGetModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
