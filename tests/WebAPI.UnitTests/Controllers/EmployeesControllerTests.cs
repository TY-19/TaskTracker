using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class EmployeesControllerTests
{
    private readonly Mock<IEmployeeService> _serviceMock;
    private readonly EmployeesController _controller;
    public EmployeesControllerTests()
    {
        _serviceMock = new Mock<IEmployeeService>();
        _controller = new EmployeesController(_serviceMock.Object);
    }
    [Fact]
    public async Task GetAllEmployee_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<EmployeeGetModel>());

        var result = (await _controller.GetAllEmployees()).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllEmployeesOfTheBoard_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetAllEmployeeFromTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<EmployeeGetModel>());

        var result = (await _controller.GetAllEmployeesOfTheBoard(1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllEmployeesOfTheBoard_ReturnsEnumerableOfEmployeeGetBoardModels()
    {
        _serviceMock.Setup(s => s.GetAllEmployeeFromTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<EmployeeGetModel>());

        var result = ((await _controller.GetAllEmployeesOfTheBoard(1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<EmployeeGetModel>>(result);
    }
    [Fact]
    public async Task GetEmployeeById_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new EmployeeGetModel());

        var result = (await _controller.GetEmployeeById(1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetEmployeeById_ReturnsEmployeeGetBoardModel()
    {
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new EmployeeGetModel());

        var result = ((await _controller.GetEmployeeById(1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<EmployeeGetModel>(result);
    }
    [Fact]
    public async Task GetEmployeeById_ReturnsNotFoundResult()
    {
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((EmployeeGetModel?)null);

        var result = (await _controller.GetEmployeeById(1)).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task AddEmployeeToTheBoard_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.AddEmployeeToTheBoardAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Callback(() => { });

        var result = await _controller.AddEmployeeToTheBoard(1, "Employee");

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task AddEmployeeToTheBoard_ReturnsBadRequestObjectResult_IfEmployeeWasNotAddedToTheBoard()
    {
        _serviceMock.Setup(s => s.AddEmployeeToTheBoardAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.AddEmployeeToTheBoard(1, "Employee");

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task RemoveEmployeeFromTheBoard_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.RemoveEmployeeFromTheBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Callback(() => { });

        var result = await _controller.RemoveEmployeeFromTheBoard(1, 1);

        Assert.IsType<NoContentResult>(result);
    }
}
