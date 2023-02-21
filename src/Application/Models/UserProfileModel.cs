namespace TaskTracker.Application.Models;
public class UserProfileModel
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IEnumerable<int> BoardsIds { get; set; } = new List<int>();
    public IEnumerable<int> AssignmentsIds { get; set; } = new List<int>();
}