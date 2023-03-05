namespace TaskTracker.Application.UnitTests.Services;

public class StageServiceTests
{
    [Fact]
    public async Task GetAllStagesOfTheBoardAsync_ReturnsAllStagesFromTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetAllStagesOfTheBoardAsync_ReturnsAnEmptyList_IfThereAreNoStagesOnTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_WorksCorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_ThrowsAnException_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AddStageToTheBoardAsync_ReturnsCorrectStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsTheCorrectStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsNull_IfStageDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetStageByIdAsync_ReturnsNull_IfStageDoesNotExistInTheProvidedBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateStageAsync_UpdatesStage_IfDataIsCorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateStageAsync_ThrowsAnException_IfTheStageDoesNotExistInTheProvidedBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteStageAsync_DeletesTheAssignment_IfProvidedDataIsValid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteStageAsync_DoesNotThrowAnException_IfThereAreNoSuchAStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task DeleteStageAsync_DoesNotDeleteTheStage_IfItIsOnOtherBoard()
    {
        throw new NotImplementedException();
    }
}
