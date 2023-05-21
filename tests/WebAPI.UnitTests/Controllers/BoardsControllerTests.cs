using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;
using TaskTracker.WebAPI.UnitTests.Helpers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class BoardsControllerTests
{
    private readonly Mock<IBoardService> _serviceMock;
    private readonly BoardsController _controller;
    public BoardsControllerTests()
    {
        _serviceMock = new Mock<IBoardService>();
        _controller = new BoardsController(_serviceMock.Object, ControllersHelper.GetValidationService());
    }
    [Fact]
    public async Task GetAllBoards_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetAllBoardsAsync())
            .ReturnsAsync(new List<BoardGetModel>());

        var result = (await _controller.GetAllBoards()).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllBoards_ReturnsEnumerableOfBoardGetModels()
    {
        _serviceMock.Setup(s => s.GetAllBoardsAsync())
            .ReturnsAsync(new List<BoardGetModel>());

        var result = ((await _controller.GetAllBoards()).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<BoardGetModel>>(result);
    }
    [Fact]
    public async Task GetBoard_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetBoardByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new BoardGetModel());

        var result = (await _controller.GetBoard(1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetBoard_ReturnsBoardGetModel()
    {
        _serviceMock.Setup(s => s.GetBoardByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new BoardGetModel());

        var result = ((await _controller.GetBoard(1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<BoardGetModel>(result);
    }
    [Fact]
    public async Task GetBoard_ReturnsNotFoundResult()
    {
        _serviceMock.Setup(s => s.GetBoardByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((BoardGetModel?)null);

        var result = (await _controller.GetBoard(1)).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task CreateNewBoard_ReturnsCreatedAtActionResult()
    {
        _serviceMock.Setup(s => s.AddBoardAsync(It.IsAny<string>()))
            .ReturnsAsync(new BoardGetModel());

        var result = await _controller.CreateNewBoard(new BoardPostModel());

        Assert.IsType<CreatedAtActionResult>(result);
    }
    [Fact]
    public async Task CreateNewBoard_ReturnsBadRequestObjectResult_IfTheBoardWasNotCreated()
    {
        _serviceMock.Setup(s => s.AddBoardAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.CreateNewBoard(new BoardPostModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task CreateNewBoard_ReturnsBadRequestObjectResult_IfTheBoardWasNoSavedToTheDatabase()
    {
        _serviceMock.Setup(s => s.AddBoardAsync(It.IsAny<string>()))
            .ReturnsAsync((BoardGetModel?)null);

        var result = await _controller.CreateNewBoard(new BoardPostModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateBoardName_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.UpdateBoardNameAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Callback(() => { });

        var result = await _controller.UpdateBoardName(1, new BoardPutModel());

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateBoardName_ReturnsBadRequestObjectResult_IfTheBoardNameWasNotUpdated()
    {
        _serviceMock.Setup(s => s.UpdateBoardNameAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.UpdateBoardName(1, new BoardPutModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task DeleteBoard_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.DeleteBoardAsync(It.IsAny<int>()))
            .Callback(() => { });

        var result = await _controller.DeleteBoard(1);

        Assert.IsType<NoContentResult>(result);
    }
}
