using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class BoardPostPutModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
