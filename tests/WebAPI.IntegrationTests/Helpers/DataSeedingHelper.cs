using Microsoft.EntityFrameworkCore;
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

    public async Task CreateBoardAsync()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Boards.AddAsync(Board1);
        await context.SaveChangesAsync();
    }
    public async Task CreateBoardAsync(Board board)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Boards.AddAsync(board);
        await context.SaveChangesAsync();
    }
    public async Task CreateStageAsync()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Stages.AddAsync(Stage1);
        await context.SaveChangesAsync();
    }
    public async Task CreateStageAsync(WorkflowStage stage)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Stages.AddAsync(stage);
        await context.SaveChangesAsync();
    }
    public async Task CreateAssignmentAsync()
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Assignments.AddAsync(Assignment1);
        await context!.SaveChangesAsync();
    }
    public async Task CreateAssignmentAsync(Assignment assignment)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Assignments.AddAsync(assignment);
        await context!.SaveChangesAsync();
    }

    public async Task CreateSubpartAsync(Subpart subpart)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Subparts.AddAsync(subpart);
        await context!.SaveChangesAsync();
    }

    public async Task CreateEmployeeAsync(Employee employee)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Employees.AddAsync(employee);
        await context!.SaveChangesAsync();
    }

    public async Task AddEmployeeToTheBoardAsync(int employeeId, int boardId)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        var board = await context!.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        var employee = await context!.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (board != null && employee != null)
        {
            employee.Boards.Add(board);
            await context.SaveChangesAsync();
        }
    }

    private Board Board1 { get; } = new Board { Id = 1 };
    private WorkflowStage Stage1 { get; } = new WorkflowStage
    {
        Id = 1,
        BoardId = 1,
        Name = "First stage",
        Position = 1
    };
    private Assignment Assignment1 { get; } = new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = 1, StageId = 1 };
}
