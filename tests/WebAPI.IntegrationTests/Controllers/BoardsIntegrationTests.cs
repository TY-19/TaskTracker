namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class BoardsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public BoardsIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsCorrectNumbersOfBoards()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_AddsBoardToTheDatabase()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsTheCorrectBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsNotFoundStatusCode_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_UpdatesBoardName()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsBadRequestStatusCode_IfNewNameIsInvalid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_DeletesBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
}
