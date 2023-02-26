using Domain.Entities;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Models;

public class EmployeeGetBoardModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
