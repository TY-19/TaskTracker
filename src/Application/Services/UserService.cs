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
        User? user = await GetUserWithEmployeeByIdOrNameFromDBAsync(userNameOrId);
        return user == null
            ? null
            : await GetUserProfileModelWithRolesAsync(user);
    }
    private async Task<User?> GetUserWithEmployeeByIdOrNameFromDBAsync(string userNameOrId)
    {
        return await GetUserWithEmployeeByIdAsync(userNameOrId) ??
            await GetUserWithEmployeeByNameAsync(userNameOrId);
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

    private async Task<User?> GetUserWithEmployeeByIdAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Boards)
            .ThenInclude(e => e.Assignments)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    private async Task<User?> GetUserWithEmployeeByNameAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Boards)
            .ThenInclude(e => e.Assignments)
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task UpdateUserNameAsync(string oldName, string newName)
    {
        User user = await _userManager.FindByNameAsync(oldName)
            ?? throw new ArgumentException("User with such a Name does not exist", nameof(oldName));
        user.UserName = newName;
        await _userManager.UpdateAsync(user);
    }

    public async Task ChangeUserPasswordAsync(string usernameOrId, string newPassword)
    {
        User user = await _userManager.FindByIdAsync(usernameOrId)
            ?? await _userManager.FindByNameAsync(usernameOrId);
        if (user == null)
            throw new ArgumentException("User with such an Id or Name does not exist", nameof(usernameOrId));
        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, newPassword);
    }

    public async Task DeleteUserAsync(string usernameOrId)
    {
        User? user = await GetUserWithEmployeeByIdOrNameFromDBAsync(usernameOrId);
        if (user == null)
            return;

        await DeleteEmployeeAsync(user.Employee);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private async Task DeleteEmployeeAsync(Employee? employee)
    {
        if (employee == null)
            return;

        Employee? toDelete = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employee.Id);

        if (toDelete == null)
            return;

        _context.Employees.Remove(toDelete);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name);
    }

    public async Task UpdateUserRoles(string userName, IEnumerable<string> roles)
    {
        User? user = await _userManager.FindByNameAsync(userName);
        if (user == null)
            return;

        IEnumerable<string> oldRoles = await _userManager.GetRolesAsync(user);
        if (!await AreToBeUpdated(oldRoles, roles))
            return;

        await _userManager.RemoveFromRolesAsync(user, oldRoles);
        await _userManager.AddToRolesAsync(user, roles);
    }

    private async Task<bool> AreToBeUpdated(IEnumerable<string> oldRoles, IEnumerable<string> newRoles)
    {
        return !AreOldAndNewRolesTheSame(oldRoles, newRoles)
            && (!await IsLastAdmin(oldRoles)
                || newRoles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE));
    }

    private static bool AreOldAndNewRolesTheSame(
        IEnumerable<string> oldRoles, IEnumerable<string> newRoles)
    {
        HashSet<string> oldRolesHashSet = new(oldRoles);
        HashSet<string> newRolesHashSet = new(newRoles);
        return oldRolesHashSet.Count == newRolesHashSet.Count
            && oldRolesHashSet.SetEquals(newRolesHashSet);
    }

    private async Task<bool> IsLastAdmin(IEnumerable<string> roles)
    {
        if (!roles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            return false;

        int adminsCount = (await _userManager.GetUsersInRoleAsync(
            DefaultRolesNames.DEFAULT_ADMIN_ROLE)).Count;

        return adminsCount == 1;
    }
}
