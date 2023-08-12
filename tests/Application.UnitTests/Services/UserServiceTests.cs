using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);

        var users = await service.GetAllUsersAsync();

        Assert.Equal(2, users.Count());
    }
    [Fact]
    public async Task GetAllUsersAsync_ReturnsAnEmptyList_IfNoUsersExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);

        var users = await service.GetAllUsersAsync();

        Assert.Empty(users);
    }
    [Fact]
    public async Task GetUserByNameOrIdAsync_ReturnsTheCorrectUser_WhenIdProvided()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);
        const string id = "12345678-1234-1234-1234-123456789012";

        var user = await service.GetUserByNameOrIdAsync(id);

        Assert.NotNull(user);
        Assert.Equal("testUser", user.UserName);
    }
    [Fact]
    public async Task GetUserByNameOrIdAsync_ReturnsTheCorrectUser_WhenNameProvided()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);
        const string userName = "secondUser";

        var user = await service.GetUserByNameOrIdAsync(userName);

        Assert.NotNull(user);
        Assert.Equal("test2@email.com", user.Email);
    }
    [Fact]
    public async Task GetUserByNameOrIdAsync_ReturnsNull_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);

        var user = await service.GetUserByNameOrIdAsync("NonExistedUser");

        Assert.Null(user);
    }
    [Fact]
    public async Task UpdateUserNameAsync_UpdatesName_IfDataIsCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);
        const string oldName = "testUser";
        const string newName = "updatedUser";
        string id = (await context.Users.FirstOrDefaultAsync(u => u.UserName == oldName))?.Id ?? string.Empty;

        await service.UpdateUserNameAsync(oldName, newName);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        Assert.NotNull(user);
        Assert.Equal(newName, user.UserName);
    }
    [Fact]
    public async Task UpdateUserNameAsync_ThrowsAnException_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);

        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
            await service.UpdateUserNameAsync("NonExistedUser", "TryToUpdate"));
    }
    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789012")]
    [InlineData("testUser")]
    public async Task DeleteUserAsync_DeletesTheUser_IfProvidedDataIsValid(string nameOrId)
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteUserAsync(nameOrId);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == nameOrId || u.UserName == nameOrId);

        Assert.Null(user);
    }
    [Fact]
    public async Task DeleteUserAsync_DeletesTheEmployee_IfUserContainsEmployee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteUserAsync("testUser");
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 1);

        Assert.Null(employee);
    }
    [Fact]
    public async Task DeleteUserAsync_DoesNotThrowAnException_IfThereAreNoSuchAnUser()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);

        var exception = await Record.ExceptionAsync(async () =>
            await service.DeleteUserAsync("testUser"));

        Assert.Null(exception);
    }
    [Fact]
    public async Task DeleteUserAsync_DoesNotThrowAnException_IfTUserDoesNotContainEmployee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var user = new User()
        {
            Id = "12345678-1234-1234-1234-123456789012",
            UserName = "testUser"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        var service = GetUserService(context);

        var exception = await Record.ExceptionAsync(async () =>
            await service.DeleteUserAsync("testUser"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);
        await DefaultData.SeedAsync(context);
        const string userName = "testUser";
        var oldHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == userName))?.PasswordHash;

        await service.ChangeUserPasswordAsync(userName, "newPassword");
        var newHash = (await context.Users.FirstOrDefaultAsync(u => u.UserName == userName))?.PasswordHash;

        Assert.Multiple(
            () => Assert.NotNull(oldHash),
            () => Assert.NotNull(newHash),
            () => Assert.NotEqual(oldHash, newHash)
        );
    }
    [Fact]
    public async Task ChangeUserPasswordAsync_ThrowsAnException_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetUserService(context);

        await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
            await service.ChangeUserPasswordAsync("NonExistedUser", "newPassword"));
    }
    [Fact]
    public async Task GetAllRoles_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var role = new IdentityRole()
        {
            Id = "12345678-1234-1234-1234-123456789012",
            Name = "TestRole",
            NormalizedName = "TESTROLE"
        };
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        var service = GetUserService(context);

        var result = service.GetAllRoles();

        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Contains("TestRole", result)
        );
    }
    [Fact]
    public async Task UpdateUserRoles_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>() { DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE });
        mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.GetUsersInRoleAsync(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            .ReturnsAsync(new List<User>() { new User(), new User() });
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
    }
    [Fact]
    public async Task UpdateUserRoles_DoesntThrowException_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
    }
    [Fact]
    public async Task UpdateUserRoles_DoesntUpdateRoles_IfOldAndNewRolesAreTheSame()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>() {
                DefaultRolesNames.DEFAULT_MANAGER_ROLE, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE });
        mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.GetUsersInRoleAsync(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            .ReturnsAsync(new List<User>() { new User(), new User() });
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
    }
    [Fact]
    public async Task UpdateUserRoles_DoesntUpdateRoles_IfUserIsTheLastAdminAndIsToBeRemovedFromAdminRole()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>() { DefaultRolesNames.DEFAULT_ADMIN_ROLE });
        mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.GetUsersInRoleAsync(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            .ReturnsAsync(new List<User>() { new User() });
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never());
    }
    [Fact]
    public async Task UpdateUserRoles_UpdatesRoles_IfUserIsTheLastAdminButIsNotToBeRemovedFromAdminRole()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>() { DefaultRolesNames.DEFAULT_ADMIN_ROLE });
        mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.GetUsersInRoleAsync(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            .ReturnsAsync(new List<User>() { new User() });
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_ADMIN_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
    }
    [Fact]
    public async Task UpdateUserRoles_UpdatesRoles_IfUserIsAdminButNotLast()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mockUserManager = ServicesTestsHelper.GetMockUserManager(context);
        mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>() { DefaultRolesNames.DEFAULT_ADMIN_ROLE });
        mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        mockUserManager.Setup(m => m.GetUsersInRoleAsync(DefaultRolesNames.DEFAULT_ADMIN_ROLE))
            .ReturnsAsync(new List<User>() { new User(), new User() });
        var service = GetUserService(context, mockUserManager.Object);

        await service.UpdateUserRolesAsync("testUser", new List<string>() {
            DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE,
            DefaultRolesNames.DEFAULT_MANAGER_ROLE
        });

        mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
        mockUserManager.Verify(m => m.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Once());
    }

    private static UserService GetUserService(TestDbContext context)
    {
        var userManager = ServicesTestsHelper.GetUserManager(context);
        return GetUserService(context, userManager);
    }

    private static UserService GetUserService(TestDbContext context, UserManager<User> userManager)
    {
        return new UserService(ServicesTestsHelper.GetMapper(), userManager,
            ServicesTestsHelper.GetRoleManager(context), context);
    }
}
