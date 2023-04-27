namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class StagesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public StagesIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
    [Fact]
    public async Task StagesController_GetAllStagesOfTheBoard_ReturnsCorrectNumbersOfStages()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_GetAllStagesOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_CreatesANewStageOnTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsTheCorrectStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsNotFoundStatusCode_IfStageDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsNotFoundStatusCode_IfStageDoesNotBelongToThisBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_UpdateStageById_UpdatesStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsBadRequest_IfBoardIdIsIncorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_DeleteStageById_DeletesStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteStageById_DoesNotDeleteTageIfItBelongsToAnotherBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsBadRequest_IfItContainsAssignmentsThatCannotBeMovedToAnotherStage()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
}
