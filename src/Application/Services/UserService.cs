using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITrackerDbContext _context;
    public UserService(IMapper mapper,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ITrackerDbContext context)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    public async Task<IEnumerable<UserProfileModel>> GetAllUsersAsync()
    {
        var users = _userManager.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Boards)
                .ThenInclude(e => e.Assignments);
        return await GetUserProfileModelsWithRolesAsync(users);
    }

    public async Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId)
    {
        var user = await GetUserByIdOrNameInnerAsync(userNameOrId);

        if (user == null)
            return null;

        return await GetUserProfileModelWithRolesAsync(user);
    }

    private async Task<User?> GetUserByIdOrNameInnerAsync(string userNameOrId)
    {
        return await GetUserByIdInnerAsync(userNameOrId) ??
            await GetUserByNameInnerAsync(userNameOrId);
    }

    private async Task<User?> GetUserByIdInnerAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Boards)
            .ThenInclude(e => e.Assignments)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    private async Task<User?> GetUserByNameInnerAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Boards)
            .ThenInclude(e => e.Assignments)
            .FirstOrDefaultAsync(u => u.UserName == userName);
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
        var user = await GetUserByIdOrNameInnerAsync(usernameOrId);
        if (user == null)
            throw new ArgumentException(
                "User with such an Id or Name does not exist", nameof(usernameOrId));

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, newPassword);
    }

    public async Task DeleteUserAsync(string usernameOrId)
    {
        var user = await GetUserByIdOrNameInnerAsync(usernameOrId);
        if (user == null)
            return;
        if (user.Employee != null)
        {
            var toDelete = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == user.Employee.Id);
            if(toDelete != null)
            {
                _context.Employees.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        await _userManager.DeleteAsync(user);
    }

    private async Task<IEnumerable<UserProfileModel>> GetUserProfileModelsWithRolesAsync(IEnumerable<User> users)
    {
        var models = new List<UserProfileModel>();
        foreach (var user in users)
        {
            models.Add(await GetUserProfileModelWithRolesAsync(user));
        }
        return models;
    }
    private async Task<UserProfileModel> GetUserProfileModelWithRolesAsync(User user)
    {
        var model = _mapper.Map<UserProfileModel>(user);
        model.Roles = await _userManager.GetRolesAsync(user);
        return model;
    }

    public IEnumerable<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name);
    }

    public async Task UpdateUserRoles(string userName, IEnumerable<string> roles)
    {
        var user = await GetUserByNameInnerAsync(userName);
        if (user == null)
            return;

        IEnumerable<string> oldRoles = await _userManager.GetRolesAsync(user);
        if (!await IsLastAdmin(oldRoles) || roles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
        {
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRolesAsync(user, roles);
        }
    }

    private async Task<bool> IsLastAdmin(IEnumerable<string> roles)
    {
        if (!roles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            return false;

        int adminCount = 0;
        foreach (var user in _context.Users)
        {
            if ((await _userManager.GetRolesAsync(user)).Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            {
                adminCount++;
                if (adminCount > 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
