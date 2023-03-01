using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure;

/// <summary>
/// Is used to seed a database with the default data
/// </summary>
public class SeedData
{
    private readonly TrackerDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public SeedData(TrackerDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Seeds the database with the default user's roles
    /// and default admin user
    /// </summary>
    /// <returns>Task to be awaited</returns>
    public async Task SeedDefaultRolesAndUsersAsync()
    {
        if (_context.Database.IsRelational())
        {
            await ApplyMigrationAsync();
        }
        await SeedDefaultRolesAsync();
        await SeedDefaultAdministratorAsync();
        await _context.SaveChangesAsync();
    }

    private async Task ApplyMigrationAsync()
    {
        if (_context.Database.GetPendingMigrationsAsync().Result.Any())
        {
            await _context.Database.MigrateAsync();
        }
    }

    private async Task SeedDefaultRolesAsync()
    {
        if (!_context.Roles.Any())
        {
            string[] defaultRoles = new string[]
            {
                DefaultRolesNames.DEFAULT_ADMIN_ROLE,
                DefaultRolesNames.DEFAULT_MANAGER_ROLE,
                DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
            };
            foreach (var defaultRole in defaultRoles)
            {
                await _roleManager.CreateAsync(new IdentityRole(defaultRole));
            }
        }
    }

    private async Task SeedDefaultAdministratorAsync()
    {
        if (!_context.Users.Any() || (_configuration["SetNewAdmin"] ?? "false") == "true")
        {
            string defaultAdminName = _configuration["DefaultCredentials:Admin:Name"]
                ?? "admin";
            string defaultAdminEmail = _configuration["DefaultCredentials:Admin:Email"]
                ?? "email@example.com";
            string defaultAdminPassword = _configuration["DefaultCredentials:Admin:Password"]
                ?? "Pa$$w0rd";
            string defaultAdminRole = DefaultRolesNames.DEFAULT_ADMIN_ROLE;

            var admin = new User
            {
                UserName = defaultAdminName,
                Email = defaultAdminEmail,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(admin, defaultAdminPassword);
            await _userManager.AddToRoleAsync(admin, defaultAdminRole);
        }
    }
}
