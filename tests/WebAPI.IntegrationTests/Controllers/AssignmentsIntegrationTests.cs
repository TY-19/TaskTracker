using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class AssignmentsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    
    public AssignmentsIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task AssignmentsController_CreateANewAssignment_AddsAssignmentToTheDatabase()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const string TOPIC = "Test";
        const int BoardId = 1;
        await CreateBoard(BoardId);
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        string RequestURI = $"api/boards/{BoardId}/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = IntegrationTestsHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(1));
        Assert.True(await DoesAssignmentWithSuchATopicExistInTheDatabase(TOPIC));
    }

    [Fact]
    public async Task AssignmentsController_CreateANewAssignment_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const int BoardId = 1;
        await CreateBoard(BoardId);
        var assignment = (AssignmentPostModel?)null;
        string RequestURI = $"api/boards/{BoardId}/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = IntegrationTestsHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        
        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateANewAssignment_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const string TOPIC = "Test";
        const int BoardId = 1;
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        string RequestURI = $"api/boards/{BoardId}/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = IntegrationTestsHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateANewAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const string TOPIC = "Test";
        const int BoardId = 1;
        await CreateBoard(BoardId);
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        string RequestURI = $"api/boards/{BoardId}/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateANewAssignment_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const string TOPIC = "Test";
        const int BoardId = 1;
        await CreateBoard(BoardId);
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        string RequestURI = $"api/boards/{BoardId}/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = IntegrationTestsHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentController_GetAllAssignmentsOfTheBoard_ReturnsCorrectNumbersOfAssignments()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const int BoardId = 1;
        await CreateBoard(BoardId);
        await CreateStage(new WorkflowStage { Id = 1, BoardId = BoardId, Name = "First stage", Position = 1 });
        await CreateAssignment(new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = BoardId, StageId = 1 });
        await CreateAssignment(new Assignment() { Id = 2, Topic = "Test assignment 2", BoardId = BoardId, StageId = 1 });
        string? token = IntegrationTestsHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        string RequestURI = $"api/boards/{BoardId}/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<AssignmentGetModel>>(httpResponse.Content.ReadAsStream());

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task AssignmentController_GetAllAssignmentsOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await IntegrationTestsHelper.SetUsersTokens(_factory);
        const int BoardId = 1;
        await CreateBoard(BoardId);
        await CreateStage(new WorkflowStage { Id = 1, BoardId = BoardId, Name = "First stage", Position = 1 });
        await CreateAssignment(new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = BoardId, StageId = 1 });
        await CreateAssignment(new Assignment() { Id = 2, Topic = "Test assignment 2", BoardId = BoardId, StageId = 1 });
        string RequestURI = $"api/boards/{BoardId}/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsTheCorrectAssignment()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsNotFoundStatusCode_IfAssignmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsNotFoundStatusCode_IfAssignmentDoesNotBelongToThisBoard()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_UpdatesAssignment()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsBadRequest_IfBoardIdIsIncorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_DeletesAssignment()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }

    private async Task CreateBoard(int boardId)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Boards.AddAsync(new Board { Id = boardId });
        await context.SaveChangesAsync();
    }
    private async Task CreateStage(WorkflowStage stage)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Stages.AddAsync(stage);
        await context.SaveChangesAsync();
    }

    private async Task CreateAssignment(Assignment assignment)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        await context!.Assignments.AddAsync(assignment);
        await context!.SaveChangesAsync();
    }

    private async Task<bool> IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Assignments.CountAsync();
    }

    private async Task<bool> DoesAssignmentWithSuchATopicExistInTheDatabase(string topic)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Assignments.FirstOrDefaultAsync(a => a.Topic == topic) != null;
    }
}

