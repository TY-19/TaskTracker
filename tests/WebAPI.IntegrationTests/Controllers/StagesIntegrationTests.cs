using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class StagesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    private readonly DataSeedingHelper _seedHelper;
    public StagesIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
        _authHelper = new AuthenticationTestsHelper(_factory);
        _seedHelper = new DataSeedingHelper(_factory);
    }
    private async Task PrepareTestFixture()
    {
        await _authHelper.ConfigureAuthenticatorAsync();
        await _seedHelper.CreateBoardAsync();
    }

    [Fact]
    public async Task StagesController_GetAllStagesOfTheBoard_ReturnsCorrectNumbersOfStages()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = "First Stage" });
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 2, BoardId = 1, Name = "Second Stage" });
        const string RequestURI = $"api/boards/1/stages";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<IEnumerable<WorkflowStageGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task StagesController_GetAllStagesOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = "First Stage" });
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 2, BoardId = 1, Name = "Second Stage" });
        const string RequestURI = $"api/boards/1/stages";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_CreatesANewStageOnTheBoard()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages";
        const string StageName = "NewStage";
        var stage = new WorkflowStagePostModel() { Name = StageName };
        var content = new StringContent(JsonSerializer.Serialize(stage),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfStagesInTheBoardAsExpectedAsync(1, 1));
        Assert.True(await DoesBoardContainsStageWithSuchANameAsync(1, StageName));
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages";
        var stage = (WorkflowStagePostModel?)null;
        var content = new StringContent(JsonSerializer.Serialize(stage),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/999/stages";
        const string StageName = "NewStage";
        var stage = new WorkflowStagePostModel() { Name = StageName };
        var content = new StringContent(JsonSerializer.Serialize(stage),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = $"api/boards/1/stages";
        const string StageName = "NewStage";
        var stage = new WorkflowStagePostModel() { Name = StageName };
        var content = new StringContent(JsonSerializer.Serialize(stage),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_CreateANewStageOnTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages";
        const string StageName = "NewStage";
        var stage = new WorkflowStagePostModel() { Name = StageName };
        var content = new StringContent(JsonSerializer.Serialize(stage),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsTheCorrectStage()
    {
        await PrepareTestFixture();
        const string StageName = "NewStage";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<WorkflowStageGetModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(StageName, result.Name);
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsNotFoundStatusCode_IfStageDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsNotFoundStatusCode_IfStageDoesNotBelongToThisBoard()
    {
        await PrepareTestFixture();
        const string StageName = "NewStage";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 2, Name = StageName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_GetStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string StageName = "NewStage";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_UpdateStageById_UpdatesStage()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldStageName = "Old Stage Name";
        const string NewStageName = "New Stage Name";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = OldStageName });
        const string RequestURI = $"api/boards/1/stages/1";
        var model = new WorkflowStagePutModel() { Name = NewStageName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesBoardContainsStageWithSuchANameAsync(1, OldStageName));
        Assert.True(await DoesBoardContainsStageWithSuchANameAsync(1, NewStageName));
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsBadRequest_IfBoardIdIsIncorrect()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldStageName = "Old Stage Name";
        const string NewStageName = "New Stage Name";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 2, Name = OldStageName });
        const string RequestURI = $"api/boards/1/stages/1";
        var model = new WorkflowStagePutModel() { Name = NewStageName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string OldStageName = "Old Stage Name";
        const string NewStageName = "New Stage Name";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = OldStageName });
        const string RequestURI = $"api/boards/1/stages/1";
        var model = new WorkflowStagePutModel() { Name = NewStageName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_UpdateStageById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string OldStageName = "Old Stage Name";
        const string NewStageName = "New Stage Name";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = OldStageName });
        const string RequestURI = $"api/boards/1/stages/1";
        var model = new WorkflowStagePutModel() { Name = NewStageName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_DeleteStageById_DeletesStage()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string StageName = "Stage One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesBoardContainsStageWithSuchANameAsync(1, StageName));
    }
    [Fact]
    public async Task StagesController_DeleteStageById_DeletesStageAndMovesAssignmentToAnoterStage_IfThereAreAnotherStage()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string StageName1 = "Stage One";
        const string StageName2 = "Stage Two";
        const string AssignmentTopic = "Assignment One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName1, Position = 1 });
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 2, BoardId = 1, Name = StageName2, Position = 2 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, BoardId = 1, StageId = 1, Topic = AssignmentTopic });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfStagesInTheBoardAsExpectedAsync(1, 1));
        Assert.False(await DoesBoardContainsStageWithSuchANameAsync(1, StageName1));
        Assert.True(await DoesBoardContainsTheAssignmentOnExpectedStageAsync(1, StageName2, AssignmentTopic));
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsBadRequest_IfItContainsAssignmentsThatCannotBeMovedToAnotherStage()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string StageName = "Stage One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName, Position = 1 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, BoardId = 1, StageId = 1, Topic = "Assignment One" });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await DoesBoardContainsStageWithSuchANameAsync(1, StageName));
    }
    [Fact]
    public async Task StagesController_DeleteStageById_DoesNotDeleteTageIfItBelongsToAnotherBoard()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        const string StageName = "Stage One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 2, Name = StageName });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await DoesBoardContainsStageWithSuchANameAsync(2, StageName));
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string StageName = "Stage One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task StagesController_DeleteStageById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string StageName = "Stage One";
        await _seedHelper.CreateStageAsync(new WorkflowStage() { Id = 1, BoardId = 1, Name = StageName });
        const string RequestURI = $"api/boards/1/stages/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }

    private async Task<bool> IsNumberOfStagesInTheBoardAsExpectedAsync(int boardId, int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Stages.Where(s => s.BoardId == boardId).CountAsync();
    }
    private async Task<bool> DoesBoardContainsStageWithSuchANameAsync(int boardId, string name)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Stages.FirstOrDefaultAsync(s => s.BoardId == boardId && s.Name == name) != null;
    }
    private async Task<bool> DoesBoardContainsTheAssignmentOnExpectedStageAsync(int boardId, string expectedStageName, string assignmentTopic)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        var stage = await context!.Stages
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Name == expectedStageName);
        return stage?.Assignments.Any(a => a.Topic == assignmentTopic) ?? false;
    }
}
