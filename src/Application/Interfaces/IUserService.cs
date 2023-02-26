using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    public IEnumerable<UserProfileModel> GetAllUsers();
    public Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId);
    public Task UpdateUserName(string oldName, string newName);
    public Task ChangeUserPassword(string usernameOrId, string newPassword);
    public Task DeleteUser(string usernameOrId);
}
