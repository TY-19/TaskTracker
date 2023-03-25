using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;

namespace TaskTracker.Application.UnitTests.Services;

public class SubpartServiceTests
{
    [Fact]
    public async Task AddSubpartToTheAssignmentAsync_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        await service.AddSubpartToTheAssignmentAsync(
            new SubpartPostPutModel() { Name = "NewPart", AssignmentId = 3 });
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 3);

        Assert.Equal(1, assignment?.Subparts.Count);
    }
    [Fact]
    public async Task AddSubpartToTheAssignmentAsync_ThrowsAnException_IfAssignmentDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddSubpartToTheAssignmentAsync(
                new SubpartPostPutModel() { Name = "NewPart", AssignmentId = 100 }));
    }
    [Fact]
    public async Task AddSubpartToTheAssignmentAsync_ReturnsCorrectSubpart()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subpart = await service.AddSubpartToTheAssignmentAsync(
            new SubpartPostPutModel() { Name = "NewPart", AssignmentId = 3 });

        Assert.NotNull(subpart);
        Assert.Equal("NewPart", subpart.Name);
    }
    [Fact]
    public async Task DeleteSubpartAsync_DeletesTheSubpart_IfProvidedDataIsValid()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteSubpartAsync(1, 1);
        var assignment = await context.Assignments.FirstOrDefaultAsync(a => a.Id == 1);

        Assert.Equal(1, assignment?.Subparts.Count);
    }
    [Fact]
    public async Task DeleteSubpartAsync_DoesNotThrowAnException_IfThereAreNoSuchASubpart()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var exception = await Record.ExceptionAsync(async () =>
            await service.DeleteSubpartAsync(1, 100));

        Assert.Null(exception);
    }
    [Fact]
    public async Task DeleteSubpartAsync_DoesNotDeleteTheSubpart_IfItIsInOtherAssignment()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        await service.DeleteSubpartAsync(1, 3);

        Assert.Equal(4, context.Subparts.Count());
    }
    [Fact]
    public async Task GetAllSubpartOfTheAssignmentAsync_ReturnsAllSubpartsOfTheAssignment()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subparts = await service.GetAllSubpartOfTheAssignmentAsync(1);

        Assert.Equal(2, subparts.Count());
    }
    [Fact]
    public async Task GetAllSubpartOfTheAssignmentAsync_ReturnsAnEmptyList_IfThereAreNoSubpartsInTheAssignment()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subparts = await service.GetAllSubpartOfTheAssignmentAsync(3);

        Assert.Empty(subparts);
    }
    [Fact]
    public async Task GetSubpartByIdAsync_ReturnsTheCorrectSubpart()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subpart = await service.GetSubpartByIdAsync(1, 1);

        Assert.NotNull(subpart);
        Assert.Equal("Part 1", subpart.Name);
    }
    [Fact]
    public async Task GetSubpartByIdAsync_ReturnsNull_IfSubpartDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subpart = await service.GetSubpartByIdAsync(1, 100);

        Assert.Null(subpart);
    }
    [Fact]
    public async Task GetSubpartByIdAsync_ReturnsNull_IfSubpartDoesNotExistInTheProvidedAssignment()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        var subpart = await service.GetSubpartByIdAsync(1, 3);

        Assert.Null(subpart);
    }
    [Fact]
    public async Task UpdateSubpartAsync_UpdatesSubpart_IfDataIsCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        await service.UpdateSubpartAsync(1, 1,
            new SubpartPostPutModel() { Name = "Updated", Description = "New description" });
        var subpart = await context.Subparts.FirstOrDefaultAsync(s => s.Id == 1);

        Assert.NotNull(subpart);
        Assert.Equal("Updated", subpart.Name);
        Assert.Equal("New description", subpart.Description);
    }
    [Fact]
    public async Task UpdateSubpartAsync_ThrowsAnException_IfTheSubpartDoesNotExistInTheProvidedAssignment()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetSubpartService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UpdateSubpartAsync(1, 3,
                new SubpartPostPutModel() { Name = "Updated", Description = "New description" }));
    }

    private static SubpartService GetSubpartService(TestDbContext context)
    {
        return new SubpartService(context, ServicesTestsHelper.GetMapper());
    }
}
