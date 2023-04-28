using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class RegistrationRequestModel
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
