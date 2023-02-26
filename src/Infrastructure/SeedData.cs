using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Common;

namespace TaskTracker.Infrastructure;

public class SeedData
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly TrackerDbContext _context;
    private readonly IConfiguration _configuration;
    public SeedData(TrackerDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
        _roleManager = roleManager;
    }
    public async Task SeedDefaultRolesAndUsers()
    {
        if (_context == null)
        {
            // TODO: Add a log record
            return;
        }
        await ApplyMigration();
        await SeedDefaultRoles();
        await SeedDefaultAdministrator();
        await _context.SaveChangesAsync();
    }

    private async Task ApplyMigration()
    {
        if (_context.Database.GetPendingMigrationsAsync().Result.Any())
        {
            await _context.Database.MigrateAsync();
        }
    }

    private async Task SeedDefaultRoles()
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

    private async Task SeedDefaultAdministrator()
    {
        if (!_context.Users.Any())
        {
            string defaultAdminName = _configuration["DefaultCredentials:Admin:Name"]
                ?? "admin";
            string defaultAdminEmail = _configuration["DefaultCredentials:Admin:Email"]
                ?? "email@exmple.com";
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
