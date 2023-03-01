using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class LoginRequestModel
{
    [Required(ErrorMessage = "Name/email is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
