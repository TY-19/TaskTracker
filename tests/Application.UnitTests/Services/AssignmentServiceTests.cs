using AutoMapper;
using Moq;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;

namespace TaskTracker.Application.UnitTests.Services;

public class AssignmentServiceTests
{
    [Fact]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsAllAssignments()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAllAssignmentsOfTheBoard_ReturnsAnEmptyList_IfNoAssignmentAreInTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task CreateAssignmentAsync_AddsAssignmentToTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsTheCorrectAssignment()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsNull_IfAssignmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAssignmentAsync_ReturnsNull_IfAssignmentDoesNotExistInTheProvidedBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateAssignmentAsync_UpdatesAssignment_IfDataIsCorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateAssignmentAsync_ThrowsAnException_IfTheAssignmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateAssignmentAsync_ThrowsAnException_IfTheAssignmentDoesNotExistInTheProvidedBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DeletesTheAssignment_IfProvidedDataIsValid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DoesNotThrowAnException_IfThereAreNoSuchAnAssignment()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteAssignmentAsync_DoesNotDeleteTheAssignment_IfItIsOnOtherBoard()
    {
        throw new NotImplementedException();
    }

    private AssignmentService GetAssignmentService()
    {
        var context = ServicesTestsHelper.GetTestDbContext();
        var mapper = new Mock<IMapper>();
        return new AssignmentService(context, mapper.Object);
    }
}
