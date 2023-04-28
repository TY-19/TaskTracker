using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;

namespace TaskTracker.WebAPI.IntegrationTests.Helpers;

internal class DataSeedingHelper
{
    private readonly CustomWebApplicationFactory _factory;

    public DataSeedingHelper(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task CreateBoard()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Boards.AddAsync(Board1);
        await context.SaveChangesAsync();
    }
    public async Task CreateBoard(Board board)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Boards.AddAsync(board);
        await context.SaveChangesAsync();
    }
    public async Task CreateStage()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Stages.AddAsync(Stage1);
        await context.SaveChangesAsync();
    }
    public async Task CreateStage(WorkflowStage stage)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Stages.AddAsync(stage);
        await context.SaveChangesAsync();
    }
    public async Task CreateAssignment()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Assignments.AddAsync(Assignment1);
        await context!.SaveChangesAsync();
    }
    public async Task CreateAssignment(Assignment assignment)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Assignments.AddAsync(assignment);
        await context!.SaveChangesAsync();
    }

    public async Task CreateSubpart(Subpart subpart)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Subparts.AddAsync(subpart);
        await context!.SaveChangesAsync();
    }

    private Board Board1 { get; } = new Board { Id = 1 };
    private WorkflowStage Stage1 { get; } = new WorkflowStage { 
        Id = 1, BoardId = 1, Name = "First stage", Position = 1 };
    private Assignment Assignment1 { get; } = new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = 1, StageId = 1 };

}
