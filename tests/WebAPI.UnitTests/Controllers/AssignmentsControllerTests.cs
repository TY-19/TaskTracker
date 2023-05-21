using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;
using TaskTracker.WebAPI.UnitTests.Helpers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class AssignmentsControllerTests
{
    private readonly Mock<IAssignmentService> _assignmentServiceMock;
    private readonly Mock<ISubpartService> _subpartServiceMock;
    private readonly AssignmentsController _controller;
    public AssignmentsControllerTests()
    {
        _assignmentServiceMock = new Mock<IAssignmentService>();
        _subpartServiceMock = new Mock<ISubpartService>();
        _controller = new AssignmentsController(_assignmentServiceMock.Object,
            _subpartServiceMock.Object, ControllersHelper.GetValidationService());
    }
    [Fact]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsOkObjectResult()
    {
        _assignmentServiceMock.Setup(a => a.GetAllAssignmentsOfTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<AssignmentGetModel>());

        var result = (await _controller.GetAllAssignmentsOfTheBoard(1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsEnumerableOfAssignmentGetModels()
    {
        _assignmentServiceMock.Setup(a => a.GetAllAssignmentsOfTheBoardAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<AssignmentGetModel>());

        var result = ((await _controller.GetAllAssignmentsOfTheBoard(1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<AssignmentGetModel>>(result);
    }
    [Fact]
    public async Task GetAssignmentById_ReturnsOkObjectResult()
    {
        _assignmentServiceMock.Setup(a => a.GetAssignmentAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AssignmentGetModel());

        var result = (await _controller.GetAssignmentById(1, 1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAssignmentById_ReturnsAssignmentGetModel()
    {
        _assignmentServiceMock.Setup(a => a.GetAssignmentAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new AssignmentGetModel());

        var result = ((await _controller.GetAssignmentById(1, 1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<AssignmentGetModel>(result);
    }
    [Fact]
    public async Task GetAssignmentById_ReturnsNotFoundResult()
    {
        _assignmentServiceMock.Setup(a => a.GetAssignmentAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((AssignmentGetModel?)null);

        var result = ((await _controller.GetAssignmentById(1, 1)).Result as OkObjectResult)?.Value;

        Assert.Null(result);
    }
    [Fact]
    public async Task CreateANewAssignment_ReturnsCreatedAtActionResult()
    {
        _assignmentServiceMock.Setup(a => a.CreateAssignmentAsync(It.IsAny<int>(), It.IsAny<AssignmentPostModel>()))
           .ReturnsAsync(new AssignmentGetModel());

        var result = await _controller.CreateANewAssignment(1, new AssignmentPostModel());

        Assert.IsType<CreatedAtActionResult>(result);
    }
    [Fact]
    public async Task CreateANewAssignment_ReturnsBadRequestObjectResult_IfTheAssignmentWasNotCreated()
    {
        _assignmentServiceMock.Setup(a => a.CreateAssignmentAsync(It.IsAny<int>(), It.IsAny<AssignmentPostModel>()))
           .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.CreateANewAssignment(1, new AssignmentPostModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateAssignmentById_ReturnsNoContentResult()
    {
        _assignmentServiceMock.Setup(a => a.UpdateAssignmentAsync(It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<AssignmentPutModel>()))
               .Callback(() => { });

        var result = await _controller.UpdateAssignmentById(1, 1, new AssignmentPutModel());

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateAssignmentById_ReturnsBadRequestObjectResult_IfTheAssignmentWasNotUpdated()
    {
        _assignmentServiceMock.Setup(a => a.UpdateAssignmentAsync(It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<AssignmentPutModel>()))
               .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.UpdateAssignmentById(1, 1, new AssignmentPutModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task DeleteAssignmentById_ReturnsNoContentResult()
    {
        _assignmentServiceMock.Setup(a => a.DeleteAssignmentAsync(It.IsAny<int>(), It.IsAny<int>()))
               .Callback(() => { });

        var result = await _controller.DeleteAssignmentById(1, 1);

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task GetAllSubpartsOfTheAssignment_ReturnsOkObjectResult()
    {
        _subpartServiceMock.Setup(s => s.GetAllSubpartOfTheAssignmentAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<SubpartGetModel>());

        var result = (await _controller.GetAllSubpartsOfTheAssignment(1, 1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllSubpartsOfTheAssignment_ReturnsEnumerableOfSubpartGetModels()
    {
        _subpartServiceMock.Setup(s => s.GetAllSubpartOfTheAssignmentAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<SubpartGetModel>());

        var result = ((await _controller.GetAllSubpartsOfTheAssignment(1, 1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<SubpartGetModel>>(result);
    }
    [Fact]
    public async Task GetSubpartById_ReturnsOkObjectResult()
    {
        _subpartServiceMock.Setup(s => s.GetSubpartByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new SubpartGetModel());

        var result = (await _controller.GetSubpartById(1, 1, 1)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetSubpartById_ReturnsSubpartGetModel()
    {
        _subpartServiceMock.Setup(s => s.GetSubpartByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new SubpartGetModel());

        var result = ((await _controller.GetSubpartById(1, 1, 1)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<SubpartGetModel>(result);
    }
    [Fact]
    public async Task GetSubpartById_ReturnsNotFoundResult()
    {
        _subpartServiceMock.Setup(s => s.GetSubpartByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((SubpartGetModel?)null);

        var result = (await _controller.GetSubpartById(1, 1, 1)).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task AddSubpartToTheAssignment_ReturnsCreatedAtActionResult()
    {
        _subpartServiceMock.Setup(s => s.AddSubpartToTheAssignmentAsync(It.IsAny<SubpartPostModel>()))
           .ReturnsAsync(new SubpartGetModel());

        var result = await _controller.AddSubpartToTheAssignment(1, 1, new SubpartPostModel() { AssignmentId = 1 });

        Assert.IsType<CreatedAtActionResult>(result);
    }
    [Fact]
    public async Task AddSubpartToTheAssignment_ReturnsBadRequestObjectResult_IfTheSubpartWasNotAdded()
    {
        _subpartServiceMock.Setup(s => s.AddSubpartToTheAssignmentAsync(It.IsAny<SubpartPostModel>()))
           .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.AddSubpartToTheAssignment(1, 1, new SubpartPostModel() { AssignmentId = 1 });

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task AddSubpartToTheAssignment_ReturnsBadRequestObjectResult_IfTaskIdIsIncorrect()
    {
        _subpartServiceMock.Setup(s => s.AddSubpartToTheAssignmentAsync(It.IsAny<SubpartPostModel>()))
           .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.AddSubpartToTheAssignment(1, 1, new SubpartPostModel() { AssignmentId = 999 });

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateSubpart_ReturnsNoContentResult()
    {
        _subpartServiceMock.Setup(s => s.UpdateSubpartAsync(It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<SubpartPutModel>()))
                .Callback(() => { });

        var result = await _controller.UpdateSubpart(1, 1, 1, new SubpartPutModel() { Name = "New name" });

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateSubpart_ReturnsBadRequestObjectResult_IfTheSubpartWasNotUpdated()
    {
        _subpartServiceMock.Setup(s => s.UpdateSubpartAsync(It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<SubpartPutModel>()))
                .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.UpdateSubpart(1, 1, 1, new SubpartPutModel() { Name = "New name" });

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task DeleteSubpart_ReturnsNoContentResult()
    {
        _subpartServiceMock.Setup(s => s.DeleteSubpartAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => { });

        var result = await _controller.DeleteSubpart(1, 1, 1);

        Assert.IsType<NoContentResult>(result);
    }
}
