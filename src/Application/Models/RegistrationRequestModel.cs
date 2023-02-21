using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class RegistrationRequestModel
{
    [Required(ErrorMessage = "Name is required")]
    public string UserName { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password's length has to be at least 8 characters")]
    public string Password { get; set; } = null!;
}
