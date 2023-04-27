using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class StageServiceTests
{
    [Fact]
    public async Task GetAllStagesOfTheBoardAsync_ReturnsAllStagesFromTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllStagesOfTheBoardAsync(1);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetAllStagesOfTheBoardAsync_ReturnsAnEmptyList_IfThereAreNoStagesOnTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await context.Boards.AddAsync(new Board() { Id = 3, Name = "EmptyBoard" });
        await context.SaveChangesAsync();

        var result = await service.GetAllStagesOfTheBoardAsync(3);

        Assert.Empty(result);
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.AddStageToTheBoardAsync(1,
            new WorkflowStagePostModel() { Name = "NewStage" });
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);

        Assert.Equal(4, context.Stages.Count());
        Assert.Equal(3, board?.Stages.Count);
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_ThrowsAnException_IfBoardDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddStageToTheBoardAsync(100,
                new WorkflowStagePostModel() { Name = "NewStage" }));
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_ReturnsCorrectStage()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var stage = await service.AddStageToTheBoardAsync(1,
            new WorkflowStagePostModel() { Name = "NewStage" });

        Assert.Multiple(
            () => Assert.Equal(4, stage.Id),
            () => Assert.Equal("NewStage", stage.Name),
            () => Assert.Equal(1, stage.BoardId),
            () => Assert.Equal(3, stage.Position)
        );
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsTheCorrectStage()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var stage = await service.GetStageByIdAsync(1, 1);

        Assert.NotNull(stage);
        Assert.Equal("Start", stage.Name);
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsNull_IfStageDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var stage = await service.GetStageByIdAsync(1, 100);

        Assert.Null(stage);
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsNull_IfStageDoesNotExistInTheProvidedBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var stage = await service.GetStageByIdAsync(2, 100);

        Assert.Null(stage);
    }
    [Fact]
    public async Task UpdateStageAsync_UpdatesStage_IfDataIsCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.UpdateStageAsync(1, 1,
            new WorkflowStagePutModel() { Name = "Updated" });
        var stage = await context.Stages.FirstOrDefaultAsync(s => s.Id == 1);

        Assert.NotNull(stage);
        Assert.Equal("Updated", stage.Name);
    }
    [Fact]
    public async Task UpdateStageAsync_ThrowsAnException_IfTheStageDoesNotExistInTheProvidedBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UpdateStageAsync(2, 1,
                new WorkflowStagePutModel() { Name = "Updated" }));
    }
    [Fact]
    public async Task DeleteStageAsync_DeletesTheAssignment_IfProvidedDataIsValid()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteStageAsync(1, 1);
        var stage = await context.Stages.FirstOrDefaultAsync(s => s.Id == 1);

        Assert.Null(stage);
        Assert.Equal(2, context.Stages.Count());
    }
    [Fact]
    public async Task DeleteStageAsync_DoesNotThrowAnException_IfThereAreNoSuchAStage()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        var exception = await Record.ExceptionAsync(async () =>
            await service.DeleteStageAsync(1, 100));

        Assert.Null(exception);
    }
    [Fact]
    public async Task DeleteStageAsync_DoesNotDeleteTheStage_IfItIsOnOtherBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteStageAsync(2, 1);
        var stage = await context.Stages.FirstOrDefaultAsync(s => s.Id == 1);

        Assert.NotNull(stage);
        Assert.Equal(3, context.Stages.Count());
    }
    [Fact]
    public async Task DeleteStageAsync_MovesAssignemntToThePreviousStage_IfItContainsAssignmentAndThereAreThePreviousStage()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteStageAsync(1, 2);
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 2);
        var previousStage = context.Stages.FirstOrDefault(s => s.Id == 1);

        Assert.NotNull(assignment);
        Assert.Equal(1, assignment.StageId);
        Assert.NotNull(previousStage);
        Assert.Equal(2, previousStage.Assignments.Count);
    }
    [Fact]
    public async Task DeleteStageAsync_MovesAssignemntToTheNextStage_IfItContainsAssignmentAndThereAreTheNextStage()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteStageAsync(1, 1);
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);
        var nextStage = context.Stages.FirstOrDefault(s => s.Id == 2);

        Assert.NotNull(assignment);
        Assert.Equal(2, assignment.StageId);
        Assert.NotNull(nextStage);
        Assert.Equal(2, nextStage.Assignments.Count);
    }
    [Fact]
    public async Task DeleteStageAsync_ThrowsAnException_IfItContainsAssignmentAndThereAreNoOtherStagesToMoveThem()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetStageService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.DeleteStageAsync(2, 3));
    }


    private static StageService GetStageService(TestDbContext context)
    {
        return new StageService(context, ServicesTestsHelper.GetMapper());
    }
}
