using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class ChangePasswordModel
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
