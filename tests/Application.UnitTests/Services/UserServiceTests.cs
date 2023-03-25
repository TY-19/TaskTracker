using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;

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
        string id = "12345678-1234-1234-1234-123456789012";

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
        string userName = "secondUser";

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
        string oldName = "testUser";
        string newName = "updatedUser";
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
    public async Task DeleteUserAsync_DoesNotThrowAnException_IfThereAreNoSuchAnUser()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
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
        string userName = "testUser";
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

    private static UserService GetUserService(TestDbContext context)
    {
        return new UserService(ServicesTestsHelper.GetMapper(),
            ServicesTestsHelper.GetUserManager(context), context);
    }
}
