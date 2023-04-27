using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class StagesControllerTests
{
    private readonly Mock<IStageService> _serviceMock;
    private readonly StagesController _controller;
    public StagesControllerTests()
    {
        _serviceMock = new Mock<IStageService>();
        _controller = new StagesController(_serviceMock.Object);
    }
    [Fact]
    public async Task GetAllStagesOfTheBoard_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(a => a.GetAllStagesOfTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<WorkflowStageGetModel>());

        var result = (await _controller.GetAllStagesOfTheBoard(1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllStagesOfTheBoard_ReturnsEnumerableOfWorkflowStageGetModels()
    {
        _serviceMock.Setup(a => a.GetAllStagesOfTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<WorkflowStageGetModel>());

        var result = ((await _controller.GetAllStagesOfTheBoard(1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<WorkflowStageGetModel>>(result);
    }
    [Fact]
    public async Task GetStageById_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(a => a.GetStageByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new WorkflowStageGetModel());

        var result = (await _controller.GetStageById(1, 1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetStageById_ReturnsWorkflowStageGetModel()
    {
        _serviceMock.Setup(a => a.GetStageByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new WorkflowStageGetModel());

        var result = ((await _controller.GetStageById(1, 1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<WorkflowStageGetModel>(result);
    }
    [Fact]
    public async Task GetStageById_ReturnsNotFoundResult()
    {
        _serviceMock.Setup(a => a.GetStageByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((WorkflowStageGetModel?)null);

        var result = (await _controller.GetStageById(1, 1)).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task CreateANewStageOnTheBoard_ReturnsCreatedAtActionResult()
    {
        _serviceMock.Setup(a => a.AddStageToTheBoardAsync(It.IsAny<int>(), It.IsAny<WorkflowStagePostModel>()))
            .ReturnsAsync(new WorkflowStageGetModel());

        var result = (await _controller.CreateANewStageOnTheBoard(1, new WorkflowStagePostModel())).Result;

        Assert.IsType<CreatedAtActionResult>(result);
    }
    [Fact]
    public async Task CreateANewStageOnTheBoard_ReturnsBadRequestObjectResult_IfTheStageWasNotCreated()
    {
        _serviceMock.Setup(a => a.AddStageToTheBoardAsync(It.IsAny<int>(), It.IsAny<WorkflowStagePostModel>()))
            .ThrowsAsync(new ArgumentException("TestExcaption"));

        var result = (await _controller.CreateANewStageOnTheBoard(1, new WorkflowStagePostModel())).Result;

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateStageById_ReturnsNoContentResult()
    {
        _serviceMock.Setup(a => a.UpdateStageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<WorkflowStagePutModel>()))
            .Callback(() => { });

        var result = await _controller.UpdateStageById(1, 1, new WorkflowStagePutModel());

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateStageById_ReturnsBadRequestObjectResult_IfTheStageWasNotUpdated()
    {
        _serviceMock.Setup(a => a.UpdateStageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<WorkflowStagePutModel>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.UpdateStageById(1, 1, new WorkflowStagePutModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task DeleteStageById_ReturnsNoContentResult()
    {
        _serviceMock.Setup(a => a.DeleteStageAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Callback(() => { });

        var result = await _controller.DeleteStageById(1, 1);

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task DeleteStageById_ReturnsBadRequestObjectResult_IfStageHasNotBeenDeleted()
    {
        _serviceMock.Setup(a => a.DeleteStageAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException("TestException"));

        var result = await _controller.DeleteStageById(1, 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
