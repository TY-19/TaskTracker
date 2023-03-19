using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;

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
    public async Task UpdateBoardNameAsync_DoesNotUpdateBoardIfNewNameIsAnEmptyString()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetBoardService(context);
        await DefaultData.SeedAsync(context);

        await service.UpdateBoardNameAsync(1, "");
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);

        Assert.NotNull(board);
        Assert.Equal("Board1", board.Name);
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
        return new BoardService(context, ServicesTestsHelper.GetMapper());
    }
}
