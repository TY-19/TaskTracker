using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ITrackerDbContext _context;
    public UserService(IMapper mapper, UserManager<User> userManager, ITrackerDbContext context)
    {
        _mapper = mapper;
        _userManager = userManager;
        _context = context;
    }
    public IEnumerable<UserProfileModel> GetAllUsers()
    {
        return _mapper.Map<IEnumerable<UserProfileModel>>(_userManager.Users);
    }

    public async Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId)
    {
        var user = await GetUserByNameOrIdInnerAsync(userNameOrId);

        if (user == null)
            return null;

        user.Employee = _context.Employees.FirstOrDefault(e => e.Id == user.EmployeeId);

        return _mapper.Map<UserProfileModel>(user);
    }

    private async Task<User?> GetUserByNameOrIdInnerAsync(string userNameOrId)
    {
        var user = await _userManager.FindByIdAsync(userNameOrId);

        if (user == null)
            user = await _userManager.FindByNameAsync(userNameOrId);

        return user;
    }

    public async Task UpdateUserName(string oldName, string newName)
    {
        var user = await _userManager.FindByNameAsync(oldName);
        user.UserName = newName;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangeUserPassword(string usernameOrId, string newPassword)
    {
        var user = await GetUserByNameOrIdInnerAsync(usernameOrId);
        if (user == null)
            return;

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, newPassword);
    }

    public async Task DeleteUser(string usernameOrId)
    {
        var user = await GetUserByNameOrIdInnerAsync(usernameOrId);
        if (user == null)
            return;

        await _userManager.DeleteAsync(user);
    }
}
