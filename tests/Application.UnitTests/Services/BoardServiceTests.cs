namespace TaskTracker.Application.UnitTests.Services;

public class BoardServiceTests
{
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsAllBoards()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsAnEmptyList_IfNoBoardsExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAllBoardsAsync_ReturnsBoardsThatIncludeInnerTypes()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByIdAsync_ReturnsTheCorrectBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByIdAsync_ReturnsNull_IfAssignmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByIdAsync_ReturnsABoardWithItsInnerTypes()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsTheCorrectBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsNull_IfAssignmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetBoardByNameAsync_ReturnsABoardWithItsInnerTypes()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddBoardAsync_AddsANewBoardToTheDatabase()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddBoardAsync_ReturnsTheAddedBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfBoardWithSuchANameAlreadyExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfProvidedNameIsEmpty()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddBoardAsync_ThrowAnException_IfProvidedNameContainsOnlyDigits()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateBoardNameAsync_UpdatesTheNameOfTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateBoardNameAsync_DoesNotUpdateBoardIfNewNameIsAnEmptyString()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteBoardAsync_DeletesBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteBoardAsync_DoesNotThrowException_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
}
