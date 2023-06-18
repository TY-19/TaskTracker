﻿using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    public Task<IEnumerable<UserProfileModel>> GetAllUsersAsync();
    public Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId);
    public Task UpdateUserNameAsync(string oldName, string newName);
    public Task ChangeUserPasswordAsync(string usernameOrId, string newPassword);
    public Task DeleteUserAsync(string usernameOrId);
}
