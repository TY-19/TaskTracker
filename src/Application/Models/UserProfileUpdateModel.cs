﻿namespace TaskTracker.Application.Models;
public class UserProfileUpdateModel : IValidatableModel
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}