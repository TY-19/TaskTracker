using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IEnumerable<UserProfileModel>> GetAllUsersAsync()
    {
        return _mapper.Map<IEnumerable<UserProfileModel>>(
            await _userManager.Users.ToListAsync());
    }

    public async Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId)
    {
        var user = await GetUserByNameOrIdInnerAsync(userNameOrId);

        if (user == null)
            return null;

        user.Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == user.EmployeeId);

        return _mapper.Map<UserProfileModel>(user);
    }

    private async Task<User?> GetUserByNameOrIdInnerAsync(string userNameOrId)
    {
        var user = await _userManager.FindByIdAsync(userNameOrId);

        if (user == null)
            user = await _userManager.FindByNameAsync(userNameOrId);

        return user;
    }

    public async Task UpdateUserNameAsync(string oldName, string newName)
    {
        var user = await _userManager.FindByNameAsync(oldName);
        if (user == null)
            throw new ArgumentException(
                "User with such a Name does not exist", nameof(oldName));

        user.UserName = newName;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangeUserPasswordAsync(string usernameOrId, string newPassword)
    {
        var user = await GetUserByNameOrIdInnerAsync(usernameOrId);
        if (user == null)
            throw new ArgumentException(
                "User with such an Id or Name does not exist", nameof(usernameOrId));

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, newPassword);
    }

    public async Task DeleteUserAsync(string usernameOrId)
    {
        var user = await GetUserByNameOrIdInnerAsync(usernameOrId);
        if (user == null)
            return;

        await _userManager.DeleteAsync(user);
    }
}
