using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text;
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
        TestAdminUserToken = await GetUserTokenAsync(testAdmin.Email, password);
        TestManagerUserToken = await GetUserTokenAsync(testManager.Email, password);
        TestEmployeeUserToken = await GetUserTokenAsync(testEmployee.Email, password);
    }
    private async Task<string?> GetUserTokenAsync(string email, string password)
    {
        using var test = _factory.Services.CreateScope();
        var accountService = test.ServiceProvider.GetService<IAccountService>();
        var response = await accountService!.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = email,
            Password = password
        });
        return response.Token;
    }
    private async Task SeedTestUsersAsync()
    {
        using var test = _factory.Services.CreateScope();
        var userManager = test.ServiceProvider.GetService<UserManager<User>>();
        if (userManager != null)
        {
            await CreateUserAsync(testAdmin, password, DefaultRolesNames.DEFAULT_ADMIN_ROLE, userManager);
            await CreateUserAsync(testManager, password, DefaultRolesNames.DEFAULT_MANAGER_ROLE, userManager);
            await CreateUserAsync(testEmployee, password, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE, userManager);
        }
    }
    private static async Task CreateUserAsync(User user, string password, string role,
        UserManager<User> userManager)
    {
        try
        {
            await userManager!.CreateAsync(user, password);
        }
        catch (Exception ex)
        {
            if (ex is not DbUpdateConcurrencyException && ex is not ArgumentException)
                throw;
        }
        try
        {
            await userManager.AddToRoleAsync(user, role);
        }
        catch (Exception ex)
        {
            if (ex is not DbUpdateConcurrencyException && ex is not ArgumentException)
                throw;
        }
    }
    
    private static readonly string password = "Pa$$w0rd";
    private static readonly User testAdmin = new ()
    {
        UserName = "testadmin",
        Email = "testadmin@example.com",
        EmailConfirmed = true,
    };
    private static readonly User testManager = new()
    {
        UserName = "testmanager",
        Email = "testmanager@example.com",
        EmailConfirmed = true,
    };
    private static readonly User testEmployee = new()
    {
        UserName = "testemployee",
        Email = "testemployee@example.com",
        EmailConfirmed = true,
        Employee = new Employee() { Id = 100, FirstName = "Test", LastName = "Employee" }
    };

    public static async Task<bool> IsTryLoginSuccessfulAync(HttpClient httpClient, string nameOrEmail, string password)
    {
        const string RequestURI = $"api/account/login";
        var loginRequest = new LoginRequestModel() { NameOrEmail = nameOrEmail, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest),
        Encoding.UTF8, "application/json");

        var httpResponse = await httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<LoginResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        return result?.Success ?? false;
    }
}
