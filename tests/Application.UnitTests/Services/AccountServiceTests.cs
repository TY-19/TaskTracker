using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Application.Validators;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class AccountServiceTests
{
    [Theory]
    [InlineData("Test")]
    [InlineData("testemail@example.com")]
    public async Task LoginAsync_Success_WhenProvidedWithCorrectCredentials(string nameOrEmail)
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);

        var result = await service.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = nameOrEmail,
            Password = "password"
        });

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginAsync_ReturnResponseWithFalse_WhenProvidedWithIncorrectCredentials()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);

        var result = await service.LoginAsync(new LoginRequestModel()
        {
            NameOrEmail = "Test",
            Password = "wrongPassword"
        });

        Assert.False(result.Success);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task RegistrationAsync_ReturnsResponsWithTrue_IfProvidedValidData()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context);

        var result = await service.RegistrationAsync(new RegistrationRequestModel()
        {
            UserName = "NewUser",
            Email = "newuser@example.com",
            Password = "password"
        });

        Assert.True(result.Success);
    }
    [Theory]
    [InlineData("NewUser", "")]
    [InlineData("", "password")]
    [InlineData(null, "password")]
    [InlineData("NewUser", null)]
    public async Task RegistrationAsync_ReturnsResponsWithFalse_IfProvidedInvalidData(string? username, string? password)
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context);

        var result = await service.RegistrationAsync(new RegistrationRequestModel()
        {
            UserName = username!,
            Email = "newuser@example.com",
            Password = password!
        });

        Assert.False(result.Success);
    }
    [Fact]
    public async Task RegistrationAsync_ReturnsResponsWithFalse_IfUserWithSuchANameAlreadyExists()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);

        var result = await service.RegistrationAsync(new RegistrationRequestModel()
        {
            UserName = "Test",
            Email = "email@example.com",
            Password = "password"
        });

        Assert.False(result.Success);
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsUserProfile_IfUserExists()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);

        var result = await service.GetUserProfileAsync("Test");

        Assert.NotNull(result);
        Assert.IsType<UserProfileModel>(result);
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsNull_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context);

        var result = await service.GetUserProfileAsync("Test");

        Assert.Null(result);
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfileWithCorrectEmployee_IfUserIsAnEmpoloyee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultEmployee: true);

        var result = await service.GetUserProfileAsync("EmployeeTest");

        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.NotNull(result.EmployeeId),
            () => Assert.NotEqual(0, result.EmployeeId),
            () => Assert.Equal("The first", result.FirstName),
            () => Assert.Equal("Employee", result.LastName)
        );

    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfileWithoutEmployee_IfUserIsNotAnEmpoloyee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultAdmin: true);

        var result = await service.GetUserProfileAsync("AdminTest");

        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.Null(result.EmployeeId),
            () => Assert.Null(result.FirstName),
            () => Assert.Null(result.LastName)
        );
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsTrueAndUpdatesUserProfile()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);
        UserProfileUpdateModel updated = new() { Email = "updated@example.com" };

        var result = await service.UpdateUserProfileAsync("Test", updated);
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test");

        Assert.True(result);
        Assert.NotNull(user);
        Assert.Equal(updated.Email, user.Email);
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsTrueAndUpdatesEmployee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);
        UserProfileUpdateModel updated = new()
        {
            FirstName = "Updated First Name",
            LastName = "Updated Last Name"
        };

        var result = await service.UpdateUserProfileAsync("Test", updated);
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test");

        Assert.True(result);
        Assert.NotNull(user);
        Assert.NotNull(user.Employee);
        Assert.Equal("Updated First Name", user.Employee.FirstName);
        Assert.Equal("Updated Last Name", user.Employee.LastName);
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsFalse_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context);
        UserProfileUpdateModel updated = new()
        {
            FirstName = "Updated First Name",
            LastName = "Updated Last Name"
        };

        var result = await service.UpdateUserProfileAsync("Test", updated);

        Assert.False(result);
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsTrueAndChangesPassword_IfProvidedWithValidData()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);
        var oldHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test"))?.PasswordHash;

        var result = await service.ChangePasswordAsync("Test",
            new ChangePasswordModel() { OldPassword = "password", NewPassword = "new12345" });
        var newHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test"))?.PasswordHash;

        Assert.True(result);
        Assert.Multiple(
            () => Assert.NotNull(oldHash),
            () => Assert.NotNull(newHash),
            () => Assert.NotEqual(oldHash, newHash)
        );
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context);

        var result = await service.ChangePasswordAsync("Test",
            new ChangePasswordModel() { OldPassword = "password", NewPassword = "new12345" });

        Assert.False(result);
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalseAndDoesNotChangePassword_IfProvidedPreviousPasswordIsNotCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = await GetAccountServiceAsync(context, seedDefaultUser: true);
        var oldHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test"))?.PasswordHash;

        var result = await service.ChangePasswordAsync("Test",
            new ChangePasswordModel() { OldPassword = "IncorrectOldPassword", NewPassword = "new12345" });
        var newHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == "Test"))?.PasswordHash;

        Assert.False(result);
        Assert.Multiple(
            () => Assert.NotNull(oldHash),
            () => Assert.NotNull(newHash),
            () => Assert.Equal(oldHash, newHash)
        );
    }

    private static async Task<AccountService> GetAccountServiceAsync(
        TestDbContext context,
        bool seedDefaultUser = false,
        bool seedDefaultEmployee = false, bool seedDefaultAdmin = false)
    {
        var validator = new RegistrationRequestModelValidator();
        var userManager = ServicesTestsHelper.GetUserManager(context);
        await AddDefaultRolesAsync(context);

        if (seedDefaultUser)
            await AddTestUserAsync(userManager);
        if (seedDefaultEmployee)
            await AddEmployeeUserAsync(userManager);
        if (seedDefaultAdmin)
            await AddAdminAsync(userManager);

        return new AccountService(userManager, GetJwtHandlerService(userManager),
            ServicesTestsHelper.GetMapper(), context, validator);
    }

    private static JwtHandlerService GetJwtHandlerService(UserManager<User> userManager)
    {
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["JwtSettings:SecurityKey"]).Returns("TheTestkeyToConfigureEncryption");
        return new JwtHandlerService(configuration.Object, userManager);
    }
    private static async Task AddDefaultRolesAsync(TestDbContext context)
    {
        var roleManager = ServicesTestsHelper.GetRoleManager(context);
        await roleManager.CreateAsync(new IdentityRole()
        {
            Name = DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
        });
        await roleManager.CreateAsync(new IdentityRole()
        {
            Name = DefaultRolesNames.DEFAULT_ADMIN_ROLE
        });
    }
    private static async Task AddTestUserAsync(UserManager<User> userManager)
    {
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Test",
            Email = "testemail@example.com"
        };
        await userManager.CreateAsync(user, "password");
    }
    private static async Task AddEmployeeUserAsync(UserManager<User> userManager)
    {
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "EmployeeTest",
            Email = "employeeuseremail@example.com",
            Employee = new Employee() { Id = 22, FirstName = "The first", LastName = "Employee" },
        };
        await userManager.CreateAsync(user, "password");
        await userManager.AddToRoleAsync(user, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
    }
    private static async Task AddAdminAsync(UserManager<User> userManager)
    {
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "AdminTest",
            Email = "adminemail@example.com"
        };
        await userManager.CreateAsync(user, "password");
        await userManager.AddToRoleAsync(user, DefaultRolesNames.DEFAULT_ADMIN_ROLE);
    }
}
