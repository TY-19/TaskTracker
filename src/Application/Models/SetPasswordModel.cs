using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class SetPasswordModel
{
    public string NewPassword { get; set; } = string.Empty;
}