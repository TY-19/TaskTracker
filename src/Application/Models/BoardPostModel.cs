using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class BoardPostModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
