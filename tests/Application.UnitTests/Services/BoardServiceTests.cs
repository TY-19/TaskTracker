using Microsoft.EntityFrameworkCore;
using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Common;

namespace TaskTracker.Application.UnitTests.Services;

public class BoardServiceTests
{
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsAllBoards()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllBoardsAsync();

        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsAnEmptyList_IfNoBoardsExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        var result = await service.GetAllBoardsAsync();

        Assert.Empty(result);
    }
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsBoardsThatIncludeInnerTypes()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllBoardsAsync();
        var board = result.FirstOrDefault(b => b.Id == 1);

        Assert.NotNull(board);
        Assert.Multiple(
            () => Assert.NotNull(board.Assignments),
            () => Assert.Equal(2, board.Assignments.Count),
            () => Assert.NotNull(board.Employees),
            () => Assert.Equal(2, board.Employees.Count),
            () => Assert.NotNull(board.Stages),
            () => Assert.Equal(2, board.Stages.Count)
        );
    }
    [Fact]
    public async Task GetBoardOfTheEmployeeAsync_ReturnsAllBoards_IfCalledByAdministrator()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        UserProfileModel userModel = new()
        {
            UserName = "admin",
            Roles = new List<string>() { DefaultRolesNames.DEFAULT_ADMIN_ROLE }
        };
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(userModel);
        var service = GetBoardService(context, userService.Object);

        var result = await service.GetBoardOfTheEmployeeAsync("admin");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetBoardOfTheEmployeeAsync_ReturnsAllBoards_IfCalledByManager()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        UserProfileModel userModel = new()
        {
            UserName = "manager",
            Roles = new List<string>() { DefaultRolesNames.DEFAULT_MANAGER_ROLE }
        };
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(userModel);
        var service = GetBoardService(context, userService.Object);

        var result = await service.GetBoardOfTheEmployeeAsync("manager");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetBoardOfTheEmployeeAsync_ReturnsOnlyAccessibleToTheEmployeeBoards()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        UserProfileModel userModel = new()
        {
            UserName = "testUser",
            Roles = new List<string>() { DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE }
        };
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(userModel);
        var service = GetBoardService(context, userService.Object);

        var result = await service.GetBoardOfTheEmployeeAsync("testUser");

        Assert.NotNull(result);
        Assert.Single(result);
    }
    [Fact]
    public async Task GetBoardOfTheEmployeeAsync_ReturnsEmployeeBoardsThatIncludeInnerTypes()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        UserProfileModel userModel = new()
        {
            UserName = "testUser",
            Roles = new List<string>() { DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE }
        };
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(userModel);
        var service = GetBoardService(context, userService.Object);

        var result = await service.GetBoardOfTheEmployeeAsync("testUser");
        var board = result.FirstOrDefault(b => b.Id == 1);

        Assert.NotNull(board);
        Assert.Multiple(
            () => Assert.NotNull(board.Assignments),
            () => Assert.Equal(2, board.Assignments.Count),
            () => Assert.NotNull(board.Employees),
            () => Assert.Equal(2, board.Employees.Count),
            () => Assert.NotNull(board.Stages),
            () => Assert.Equal(2, board.Stages.Count)
        );
    }
    [Fact]
    public async Task GetBoardOfTheEmployeeAsync_ReturnsEmptyList_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserProfileModel?)null);
        var service = GetBoardService(context, userService.Object);

        var result = await service.GetBoardOfTheEmployeeAsync("notUser");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBoardByIdAsync_ReturnsTheCorrectBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var board = await service.GetBoardByIdAsync(1);

        Assert.NotNull(board);
        Assert.Equal("Board1", board.Name);
    }
    [Fact]
    public async Task GetBoardByIdAsync_ReturnsNull_IfAssignmentDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        var board = await service.GetBoardByIdAsync(100);

        Assert.Null(board);
    }
    [Fact]
    public async Task GetBoardByIdAsync_ReturnsABoardWithItsInnerTypes()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var board = await service.GetBoardByIdAsync(1);

        Assert.NotNull(board);
        Assert.Multiple(
            () => Assert.NotNull(board.Assignments),
            () => Assert.Equal(2, board.Assignments.Count),
            () => Assert.NotNull(board.Employees),
            () => Assert.Equal(2, board.Employees.Count),
            () => Assert.NotNull(board.Stages),
            () => Assert.Equal(2, board.Stages.Count)
        );
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsTheCorrectBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var board = await service.GetBoardByNameAsync("Board1");

        Assert.NotNull(board);
        Assert.Equal(1, board.Id);
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsNull_IfBoardDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        var board = await service.GetBoardByNameAsync("NonExistedBoard");

        Assert.Null(board);
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsABoardWithItsInnerTypes()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var board = await service.GetBoardByNameAsync("Board1");

        Assert.NotNull(board);
        Assert.Multiple(
            () => Assert.NotNull(board.Assignments),
            () => Assert.Equal(2, board.Assignments.Count),
            () => Assert.NotNull(board.Employees),
            () => Assert.Equal(2, board.Employees.Count),
            () => Assert.NotNull(board.Stages),
            () => Assert.Equal(2, board.Stages.Count)
        );
    }
    [Fact]
    public async Task AddBoardAsync_AddsANewBoardToTheDatabase()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        await service.AddBoardAsync("Board3");

        Assert.Equal(1, context.Boards.Count());
    }
    [Fact]
    public async Task AddBoardAsync_ReturnsTheAddedBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        var board = await service.AddBoardAsync("Board3");

        Assert.NotNull(board);
        Assert.Equal("Board3", board.Name);
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfBoardWithSuchANameAlreadyExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddBoardAsync("Board1"));
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfProvidedNameIsEmpty()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddBoardAsync(""));
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfProvidedNameContainsOnlyDigits()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddBoardAsync("12345"));
    }
    [Fact]
    public async Task UpdateBoardNameAsync_UpdatesTheNameOfTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await service.UpdateBoardNameAsync(1, "Updated");
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);

        Assert.NotNull(board);
        Assert.Equal("Updated", board.Name);
    }
    [Fact]
    public async Task UpdateBoardNameAsync_DoesNotUpdateBoard_IfNewNameIsAnEmptyString()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UpdateBoardNameAsync(1, ""));
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);

        Assert.NotNull(board);
        Assert.Equal("Board1", board.Name);
    }
    [Fact]
    public async Task UpdateBoardNameAsync_ThrowsException_IfBoardDoesntExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UpdateBoardNameAsync(1, "NewName"));
    }
    [Fact]
    public async Task DeleteBoardAsync_DeletesBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteBoardAsync(1);

        Assert.Equal(1, context.Boards.Count());
    }
    [Fact]
    public async Task DeleteBoardAsync_DoesNotThrowException_IfBoardDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        var exception = await Record.ExceptionAsync(async () =>
            await service.DeleteBoardAsync(100));

        Assert.Null(exception);
    }

    private static BoardService GetBoardService(TestDbContext context)
    {
        var userService = new Mock<IUserService>();
        return GetBoardService(context, userService.Object);
    }
    private static BoardService GetBoardService(TestDbContext context, IUserService userService)
    {
        return new BoardService(context, ServicesTestsHelper.GetMapper(), userService);
    }
}
