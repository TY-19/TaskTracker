using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class EmployeeServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetAllAsync_ReturnsAnEmptyList_IfThereAreNoEmployeesInTheDatabase()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }
    [Fact]
    public async Task GetAllEmployeeFromTheBoardAsync_ReturnsAllEmployeesFromTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllEmployeeFromTheBoardAsync(1);

        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task GetAllEmployeeFromTheBoardAsync_ReturnsAnEmptyList_IfThereAreNoEmployeesOnTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetAllEmployeeFromTheBoardAsync(100);

        Assert.Empty(result);
    }
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsTheCorrectEmployee()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var result = await service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.Equal("Test", result.FirstName),
            () => Assert.Equal("Employee", result.LastName)
        );
    }
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsNull_IfEmployeeDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);

        var result = await service.GetEmployeeByIdAsync(100);

        Assert.Null(result);
    }
    [Fact]
    public async Task AddEmployeeToTheBoardAsync_WorksCorrect()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        await service.AddEmployeeToTheBoardAsync(2, "12345678-1234-1234-1234-123456789012");
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 2);

        Assert.Equal(1, board?.Employees.Count);
    }
    [Fact]
    public async Task AddEmployeeToTheBoardAsync_CreatesANewEmployeeForTheUser_IfUserHasNoEmployeePreviously()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);
        context.Users.Add(new User() { Id = "87654321-4321-4321-4321-210987654321" });
        await context.SaveChangesAsync();
        
        await service.AddEmployeeToTheBoardAsync(2, "87654321-4321-4321-4321-210987654321");
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 2);
        var employee = board?.Employees.FirstOrDefault();

        Assert.NotNull(employee);
    }
    [Fact]
    public async Task AddEmployeeToTheBoardAsync_ThrowsAnException_IfBoardDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);
        
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddEmployeeToTheBoardAsync(100, "12345678-1234-1234-1234-123456789012"));
    }
    [Fact]
    public async Task AddEmployeeToTheBoardAsync_ThrowsAnException_IfUserDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.AddEmployeeToTheBoardAsync(2, "87654321-4321-4321-4321-210987654321"));
    }
    [Fact]
    public async Task RemoveEmployeeFromTheBoardAsync_RemovesAnEmployeeFromTheBoard()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        await service.RemoveEmployeeFromTheBoardAsync(1, 2);
        var board = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);

        Assert.Equal(1, board?.Employees.Count);
    }
    [Fact]
    public async Task RemoveEmployeeFromTheBoardAsync_DoesNotThrowAnException_IfBoardDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var exception = await Record.ExceptionAsync(
            async () => await service.RemoveEmployeeFromTheBoardAsync(100, 2));

        Assert.Null(exception);
    }
    [Fact]
    public async Task RemoveEmployeeFromTheBoardAsync_DoesNotThrowAnException_IfEmployeeDoesNotExist()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var service = GetEmployeeService(context);
        await DefaultData.SeedAsync(context);

        var exception = await Record.ExceptionAsync(
            async () => await service.RemoveEmployeeFromTheBoardAsync(1, 100));

        Assert.Null(exception);
    }

    private EmployeeService GetEmployeeService(TestDbContext context)
    {
        return new EmployeeService(context, ServicesTestsHelper.GetMapper());
    }
}
