using Domain.Entities;

namespace TaskTracker.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
