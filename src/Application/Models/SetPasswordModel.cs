using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class SetPasswordModel
{
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}