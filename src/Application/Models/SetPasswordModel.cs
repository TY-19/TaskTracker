namespace TaskTracker.Application.Models;

public class SetPasswordModel : IValidatableModel
{
    public string NewPassword { get; set; } = string.Empty;
}