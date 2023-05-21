namespace TaskTracker.Application.Models;

public class LoginRequestModel : IValidatableModel
{
    public string NameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
