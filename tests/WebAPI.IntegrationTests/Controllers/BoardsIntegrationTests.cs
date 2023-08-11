using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class BoardsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    private readonly DataSeedingHelper _seedHelper;
    public BoardsIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
        _authHelper = new AuthenticationTestsHelper(_factory);
        _seedHelper = new DataSeedingHelper(_factory);
    }

    private async Task PrepareTestFixture()
    {
        await _authHelper.ConfigureAuthenticatorAsync();
    }

    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsCorrectNumbersOfBoards()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        const string RequestURI = $"api/boards/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<IEnumerable<BoardGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        const string RequestURI = $"api/boards/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_GetAllBoards_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        const string RequestURI = $"api/boards/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_GetBoardsOfTheEmployee_ReturnsAllBoards_IfCalledByManager()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        const string RequestURI = $"api/boards/accessible";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<IEnumerable<BoardGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task BoardsController_GetBoardsOfTheEmployee_ReturnsOnlyAccessibleBoards_IfCalledByEmployee()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        const string RequestURI = $"api/boards/accessible";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<IEnumerable<BoardGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Single(result);
    }
    [Fact]
    public async Task BoardsController_GetBoardsOfTheEmployee_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = "Board1" });
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2, Name = "Board2" });
        const string RequestURI = $"api/boards/accessible";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_AddsBoardToTheDatabase()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/";
        const string BoardName = "NewBoard";
        var board = new BoardPostModel() { Name = BoardName };
        var content = new StringContent(JsonSerializer.Serialize(board),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfBoardsInTheDatabaseAsExpectedAsync(1));
        Assert.True(await DoesBoardWithSuchANameExistInTheDatabaseAsync(BoardName));
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsBadRequestStatusCode_IfBoardWithSuchNameAlreadyExist()
    {
        const string BoardName = "NewBoard";
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Name = BoardName };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/";
        var boardModel = new BoardPostModel() { Name = BoardName };
        var content = new StringContent(JsonSerializer.Serialize(boardModel),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/";
        var board = new BoardPostModel();
        var content = new StringContent(JsonSerializer.Serialize(board),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = $"api/boards/";
        const string BoardName = "NewBoard";
        var board = new BoardPostModel() { Name = BoardName };
        var content = new StringContent(JsonSerializer.Serialize(board),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_CreateNewBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/";
        const string BoardName = "NewBoard";
        var board = new BoardPostModel() { Name = BoardName };
        var content = new StringContent(JsonSerializer.Serialize(board),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsTheCorrectBoard()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string BoardName = "Board1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = BoardName });
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<BoardGetModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(BoardName, result.Name);
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsNotFoundStatusCode_IfBoardDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_GetBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string BoardName = "Board1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = BoardName });
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_UpdatesBoardName()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldName = "OldName";
        const string NewName = "NewName";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = OldName });
        const string RequestURI = $"api/boards/1";
        var model = new BoardPutModel() { Name = NewName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesBoardWithSuchANameExistInTheDatabaseAsync(OldName));
        Assert.True(await DoesBoardWithSuchANameExistInTheDatabaseAsync(NewName));
    }
    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public async Task BoardsController_UpdateBoardName_ReturnsBadRequestStatusCode_IfNewNameIsInvalid(string newName)
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldName = "OldName";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = OldName });
        const string RequestURI = $"api/boards/1";
        var model = new BoardPutModel() { Name = newName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1";
        var model = new BoardPutModel() { Name = "NewName" };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string OldName = "OldName";
        const string NewName = "NewName";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = OldName });
        const string RequestURI = $"api/boards/1";
        var model = new BoardPutModel() { Name = NewName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_UpdateBoardName_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldName = "OldName";
        const string NewName = "NewName";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = OldName });
        const string RequestURI = $"api/boards/1";
        var model = new BoardPutModel() { Name = NewName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_DeletesBoard()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string Name = "Board1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = Name });
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfBoardsInTheDatabaseAsExpectedAsync(0));
        Assert.False(await DoesBoardWithSuchANameExistInTheDatabaseAsync(Name));
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string Name = "Board1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = Name });
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task BoardsController_DeleteBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string Name = "Board1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 1, Name = Name });
        const string RequestURI = $"api/boards/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }

    private async Task<bool> IsNumberOfBoardsInTheDatabaseAsExpectedAsync(int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Boards.CountAsync();
    }

    private async Task<bool> DoesBoardWithSuchANameExistInTheDatabaseAsync(string name)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Boards.FirstOrDefaultAsync(a => a.Name == name) != null;
    }
}
