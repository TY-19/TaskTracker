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
            await _userManager.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e!.Boards)
                .ThenInclude(e => e.Assignments)
                .ToListAsync()
            );
    }

    public async Task<UserProfileModel?> GetUserByNameOrIdAsync(string userNameOrId)
    {
        var user = await GetUserByIdOrNameInnerAsync(userNameOrId);

        if (user == null)
            return null;

        return _mapper.Map<UserProfileModel>(user);
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

        await _userManager.DeleteAsync(user);
    }
}
