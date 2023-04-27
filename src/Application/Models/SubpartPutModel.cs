using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Models;

public class SubpartPutModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? PercentValue { get; set; }
}
