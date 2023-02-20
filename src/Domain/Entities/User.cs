using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Domain.Entities;

public class User : IdentityUser
{
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
