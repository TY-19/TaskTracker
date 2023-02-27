using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Domain.Entities;

/// <summary>
/// Represents an user of the application.
/// </summary>
public class User : IdentityUser
{
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
