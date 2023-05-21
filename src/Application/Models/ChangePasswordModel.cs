namespace TaskTracker.Application.Models;

public class ChangePasswordModel : IValidatableModel
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
