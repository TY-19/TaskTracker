using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class RegistrationRequestModel
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Username length has to be at least 3 characters")]
    public string UserName { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password's length has to be at least 8 characters")]
    public string Password { get; set; } = null!;
}
