namespace TaskTracker.Application.Models;
public class LoginResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}