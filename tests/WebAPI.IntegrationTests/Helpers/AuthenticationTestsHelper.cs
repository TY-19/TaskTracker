using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.WebAPI.IntegrationTests.Helpers;

internal class AuthenticationTestsHelper
{
    private readonly CustomWebApplicationFactory _factory;
    public string? TestAdminUserToken { get; private set; }
    public string? TestManagerUserToken { get; private set; }
    public string? TestEmployeeUserToken { get; private set; }

    public AuthenticationTestsHelper(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task ConfigureAuthenticatorAsync()
    {
        await SeedTestUsersAsync();
        await SetUsersTokensAsync();
    }

    private async Task SetUsersTokensAsync()
    {
        TestAdminUserToken = await GetAdminUserTokenAsync();
        TestManagerUserToken = await GetManagerUserTokenAsync();
        TestEmployeeUserToken = await GetEmployeeUserTokenAsync();
    }
    private async Task<string?> GetEmployeeUserTokenAsync()
    {
        using var test = _factory.Services.CreateScope();
        var accountService = test.ServiceProvider.GetService<IAccountService>();
        var response = await accountService!.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = testEmployee.user.Email,
            Password = testEmployee.password
        });
        return response.Token;
    }
    private async Task<string?> GetManagerUserTokenAsync()
    {
        using var test = _factory.Services.CreateScope();
        var accountService = test.ServiceProvider.GetService<IAccountService>();
        var response = await accountService!.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = testManager.user.Email,
            Password = testManager.password
        });
        return response.Token;
    }
    private async Task<string?> GetAdminUserTokenAsync()
    {
        using var test = _factory.Services.CreateScope();
        var accountService = test.ServiceProvider.GetService<IAccountService>();
        var response = await accountService!.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = testAdmin.user.Email,
            Password = testAdmin.password
        });
        return response.Token;
    }

    private async Task SeedTestUsersAsync()
    {
        using var test = _factory.Services.CreateScope();
        var userManager = test.ServiceProvider.GetService<UserManager<User>>();
        foreach (var (user, password, role) in testUsers)
        {
            await userManager!.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static readonly (User user, string password, string role) testAdmin = (
        new User()
        {
            UserName = "testadmin",
            Email = "testadmin@example.com",
            EmailConfirmed = true
        },
        "Pa$$w0rd",
        DefaultRolesNames.DEFAULT_ADMIN_ROLE
    );
    private static readonly (User user, string password, string role) testManager = (
        new User()
        {
            UserName = "testmanager",
            Email = "testmanager@example.com",
            EmailConfirmed = true
        },
        "Pa$$w0rd",
        DefaultRolesNames.DEFAULT_MANAGER_ROLE
    );
    private static readonly (User user, string password, string role) testEmployee = (
        new User()
        {
            UserName = "testemployee",
            Email = "testemployee@example.com",
            EmailConfirmed = true
        },
        "Pa$$w0rd",
        DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
    );

    private static readonly (User user, string password, string role)[] testUsers = new[]
    {
        testAdmin,
        testManager,
        testEmployee
    };


}
