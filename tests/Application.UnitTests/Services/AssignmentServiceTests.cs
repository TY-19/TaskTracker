using Microsoft.EntityFrameworkCore;
using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class AssignmentServiceTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsAllAssignments(int boardId, int expectedCount)
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        var result = await service.GetAllAssignmentsOfTheBoardAsync(boardId);

        Assert.Equal(expectedCount, result.Count());
    }
    [Fact]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsAnEmptyList_IfNoAssignmentAreInTheBoard()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await context.Boards.AddAsync(new Board { Id = 1, Name = "Board1" });
        AssignmentService service = GetAssignmentService(context);

        var result = await service.GetAllAssignmentsOfTheBoardAsync(1);

        Assert.Empty(result);
    }
    [Fact]
    public async Task CreateAssignmentAsync_AddsAssignmentToTheBoard()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        var board = new Board { Name = "Board1" };
        var stage = new WorkflowStage { Name = "Stage1", Position = 1, BoardId = board.Id, };
        var assignment = new AssignmentPostModel() { Topic = "Task 1", StageId = stage.Id };
        await context.Boards.AddAsync(board);
        await context.Stages.AddAsync(stage);
        await context.SaveChangesAsync();
        AssignmentService service = GetAssignmentService(context);

        await service.CreateAssignmentAsync(board.Id, assignment);
        var result = await context.Assignments.FirstOrDefaultAsync(a => a.Topic == "Task 1");

        Assert.NotNull(result);
        Assert.Equal(board.Id, result.BoardId);
        Assert.Equal(board.Name, result.Board.Name);
    }
    [Fact]
    public async Task CreateAssignmentAsync_ThrowsArgumentExceptionIfBoardDoesNotExist()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        var assignment = new AssignmentPostModel() { Topic = "Task 1", StageId = 1 };
        AssignmentService service = GetAssignmentService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.CreateAssignmentAsync(1, assignment));
    }
    [Fact]
    public async Task CreateAssignmentAsync_ThrowsArgumentExceptionIfModelIsNull()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        var board = new Board { Name = "Board1" };
        var stage = new WorkflowStage { Name = "Stage1", Position = 1, BoardId = board.Id, };
        await context.Boards.AddAsync(board);
        await context.Stages.AddAsync(stage);
        await context.SaveChangesAsync();
        AssignmentService service = GetAssignmentService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.CreateAssignmentAsync(1, null!));
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsTheCorrectAssignment()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        var result = await service.GetAssignmentAsync(1, 1);

        Assert.NotNull(result);
        Assert.Equal("First Assignment", result.Topic);
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsNull_IfAssignmentDoesNotExist()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        var result = await service.GetAssignmentAsync(1, 100);

        Assert.Null(result);
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsNull_IfAssignmentDoesNotExistInTheProvidedBoard()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        var result = await service.GetAssignmentAsync(1, 3);

        Assert.Null(result);
    }
    [Fact]
    public async Task UpdateAssignmentAsync_UpdatesAssignment_IfDataIsCorrect()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);
        string topic = "Updated";
        var model = new AssignmentPutModel() { Topic = topic };

        await service.UpdateAssignmentAsync(1, 1, model);
        var result = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1 && a.BoardId == 1);

        Assert.Equal(topic, result?.Topic);
    }
    [Fact]
    public async Task UpdateAssignmentAsync_ThrowsAnException_IfTheAssignmentDoesNotExist()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);
        string topic = "Updated";
        var model = new AssignmentPutModel() { Topic = topic };

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await service.UpdateAssignmentAsync(1, 100, model));
    }
    [Fact]
    public async Task UpdateAssignmentAsync_ThrowsAnException_IfTheAssignmentDoesNotExistInTheProvidedBoard()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);
        string topic = "Updated";
        var model = new AssignmentPutModel() { Topic = topic };

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await service.UpdateAssignmentAsync(1, 3, model));
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DeletesTheAssignment_IfProvidedDataIsValid()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);
        var seeded = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        await service.DeleteAssignmentAsync(1, 1);
        var deleted = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(seeded);
        Assert.Null(deleted);
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DoesNotThrowAnException_IfThereAreNoSuchAnAssignment()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        var exception = await Record.ExceptionAsync(
            async () => await service.DeleteAssignmentAsync(1, 1000));

        Assert.Null(exception);
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DoesNotDeleteTheAssignment_IfItIsOnOtherBoard()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        await service.DeleteAssignmentAsync(2, 1);
        var result = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task MoveAssignmentToTheStageAsync_MovesAssignmentToTheDestinatedStage_IfCalledByResponsibleEmployee()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel() { EmployeeId = 1 });
        AssignmentService service = GetAssignmentService(context, userService.Object);

        await service.MoveAssignmentToTheStageAsync(1, 1, 2, "testUser");
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.Equal(2, assignment.StageId);
    }
    [Fact]
    public async Task MoveAssignmentToTheStageAsync_MovesAssignmentToTheDestinatedStage_IfCalledByManager()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel()
            {
                Roles = new List<string>() { DefaultRolesNames.DEFAULT_MANAGER_ROLE }
            });
        AssignmentService service = GetAssignmentService(context, userService.Object);

        await service.MoveAssignmentToTheStageAsync(1, 1, 2, "testUser");
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.Equal(2, assignment.StageId);
    }
    [Fact]
    public async Task MoveAssignmentToTheStageAsync_ThrowsExceptionAndDoesntMoveAssignment_IfUserHasNoPermission()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.MoveAssignmentToTheStageAsync(1, 1, 2, "nonExistedUser"));
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.Equal(1, assignment.StageId);
    }
    [Fact]
    public async Task ChangeAssignmentStatus_ChangesStatus_IfCalledByResponsibleEmployee()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel() { EmployeeId = 1 });
        AssignmentService service = GetAssignmentService(context, userService.Object);

        await service.ChangeAssignmentStatus(1, 1, true, "testUser");
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.True(assignment.IsCompleted);
    }
    [Fact]
    public async Task ChangeAssignmentStatus_ChangesStatus_IfCalledByAdmin()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel()
            {
                Roles = new List<string>() { DefaultRolesNames.DEFAULT_ADMIN_ROLE }
            });
        AssignmentService service = GetAssignmentService(context, userService.Object);

        await service.ChangeAssignmentStatus(1, 1, true, "testUser");
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.True(assignment.IsCompleted);
    }
    [Fact]
    public async Task ChangeAssignmentStatus_ThrowsExceptionAndDoesntChangeStatus_IfUserHasNoPermission()
    {
        using var context = ServicesTestsHelper.GetTestDbContext();
        await DefaultData.SeedAsync(context);
        AssignmentService service = GetAssignmentService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.ChangeAssignmentStatus(1, 1, true, "nonExistedUser"));
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.NotNull(assignment);
        Assert.False(assignment.IsCompleted);
    }

    private static AssignmentService GetAssignmentService(TestDbContext context)
    {
        var userService = new Mock<IUserService>();
        return GetAssignmentService(context, userService.Object);
    }

    private static AssignmentService GetAssignmentService(TestDbContext context, IUserService userService)
    {
        var mapper = ServicesTestsHelper.GetMapper();
        return new AssignmentService(context, mapper, userService);
    }
}
