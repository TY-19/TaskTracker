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

public class AssignmentsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    private readonly DataSeedingHelper _seedHelper;

    public AssignmentsIntegrationTests()
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
        await _seedHelper.CreateStageAsync();
    }

    [Fact]
    public async Task AssignmentsController_CreateNewAssignment_AddsAssignmentToTheDatabase()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test";

        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });

        var assignment = new AssignmentPostModel()
        {
            Topic = TOPIC,
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1000
        };
        const string RequestURI = $"api/boards/1/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(1));
        Assert.True(await DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(TOPIC));
    }

    [Fact]
    public async Task AssignmentsController_CreateNewAssignment_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        var assignment = new AssignmentPostModel();
        const string RequestURI = $"api/boards/1/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateNewAssignment_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test";
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        const string RequestURI = $"api/boards/999/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateNewAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test";
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        const string RequestURI = $"api/boards/1/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentsController_CreateNewAssignment_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test";
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1 };
        const string RequestURI = $"api/boards/1/tasks";
        var content = new StringContent(JsonSerializer.Serialize(assignment),
            Encoding.UTF8, "application/json");
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(0));
    }

    [Fact]
    public async Task AssignmentController_GetAllAssignmentsOfTheBoard_ReturnsCorrectNumbersOfAssignments()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = 1, StageId = 1, ResponsibleEmployeeId = 1000 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 2, Topic = "Test assignment 2", BoardId = 1, StageId = 1, ResponsibleEmployeeId = 1000 });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<AssignmentGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task AssignmentController_GetAllAssignmentsOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = "Test assignment 1", BoardId = 1, StageId = 1 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 2, Topic = "Test assignment 2", BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsTheCorrectAssignment()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 1, StageId = 1, ResponsibleEmployeeId = 1000 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<AssignmentGetModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(TOPIC, result.Topic);
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsNotFoundStatusCode_IfAssignmentDoesNotExist()
    {
        await PrepareTestFixture();
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsNotFoundStatusCode_IfAssignmentDoesNotBelongToThisBoard()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 2, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_UpdatesAssignment()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = OLD_TOPIC, BoardId = 1, StageId = 1, ResponsibleEmployeeId = 1000 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(OLD_TOPIC));
        Assert.True(await DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(NEW_TOPIC));
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = "Topic", BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = string.Empty };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, ((int)httpResponse.StatusCode));
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsBadRequest_IfBoardIdIsIncorrect()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = OLD_TOPIC, BoardId = 2, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, ((int)httpResponse.StatusCode));
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = OLD_TOPIC, BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, ((int)httpResponse.StatusCode));
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = OLD_TOPIC, BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, ((int)httpResponse.StatusCode));
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_DeletesAssignment()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 1, StageId = 1, ResponsibleEmployeeId = 1000 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(TOPIC));
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 1, Topic = TOPIC, BoardId = 1, StageId = 1 });
        const string RequestURI = $"api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, ((int)httpResponse.StatusCode));
    }

    private async Task<bool> IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Assignments.CountAsync();
    }

    private async Task<bool> DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(string topic)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Assignments.FirstOrDefaultAsync(a => a.Topic == topic) != null;
    }
}

