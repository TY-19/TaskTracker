using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;
public class UserProfileUpdateModel
{
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}