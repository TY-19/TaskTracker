using Domain.Entities;

namespace TaskTracker.Domain.Entities;

/// <summary>
/// Represents an employee who works on tasks
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public User? User { get; set; }
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
