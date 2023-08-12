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
        const string RequestURI = "api/boards/1/tasks";
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
        const string RequestURI = "api/boards/1/tasks";
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
        var assignment = new AssignmentPostModel()
        {
            Topic = TOPIC,
            StageId = 1,
            Deadline = DateTime.MaxValue,
            ResponsibleEmployeeId = 1
        };
        const string RequestURI = "api/boards/999/tasks";
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
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1, Deadline = DateTime.MaxValue, ResponsibleEmployeeId = 1 };
        const string RequestURI = "api/boards/1/tasks";
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
        var assignment = new AssignmentPostModel() { Topic = TOPIC, StageId = 1, Deadline = DateTime.MaxValue, ResponsibleEmployeeId = 1 };
        const string RequestURI = "api/boards/1/tasks";
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
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 1, responsibleEmployeeId: 1000));
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 2, responsibleEmployeeId: 1000));
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/tasks";

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
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 1));
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 2));
        const string RequestURI = "api/boards/1/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetAllAssignmentsOfTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeThatIsNotPartOfBoard()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 1, responsibleEmployeeId: 1000));
        await _seedHelper.CreateAssignmentAsync(GetAssignment(id: 2, responsibleEmployeeId: 1000));
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/tasks";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsTheCorrectAssignment()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000));
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        const string RequestURI = "api/boards/1/tasks/1";
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
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsNotFoundStatusCode_IfAssignmentDoesNotBelongToThisBoard()
    {
        await PrepareTestFixture();
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";
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
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeThatIsNotPartOfBoard()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000));
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_UpdatesAssignment()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: OLD_TOPIC, responsibleEmployeeId: 1000));
        const string RequestURI = "api/boards/1/tasks/1";
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
        await _seedHelper.CreateAssignmentAsync(GetAssignment());
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = string.Empty };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsBadRequest_IfBoardIdIsIncorrect()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateBoardAsync(new Board() { Id = 2 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: OLD_TOPIC, boardId: 2));
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: OLD_TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_UpdateAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string OLD_TOPIC = "Old topic";
        const string NEW_TOPIC = "New topic";
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: OLD_TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new AssignmentPutModel() { Topic = NEW_TOPIC };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_MoveAssignmentToTheStage_MovesAssignment_IfCalledByManager()
    {
        const string TOPIC = "Topic";
        const int DESTINATION_STAGE_ID = 2;
        await PrepareTestFixture();
        await _seedHelper.CreateStageAsync(new WorkflowStage
        {
            Id = DESTINATION_STAGE_ID,
            BoardId = 1,
            Name = "Second stage",
            Position = 2
        });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100));
        string RequestURI = "api/boards/1/tasks/1/move/" + DESTINATION_STAGE_ID;
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.Equal(DESTINATION_STAGE_ID, assignmnet.StageId);
    }
    [Fact]
    public async Task AssignmentController_MoveAssignmentToTheStage_MovesAssignment_IfCalledByResponsibleEmployee()
    {
        const string TOPIC = "Topic";
        const int DESTINATION_STAGE_ID = 2;
        await PrepareTestFixture();
        await _seedHelper.CreateStageAsync(new WorkflowStage
        {
            Id = DESTINATION_STAGE_ID,
            BoardId = 1,
            Name = "Second stage",
            Position = 2
        });
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100));
        string RequestURI = "api/boards/1/tasks/1/move/" + DESTINATION_STAGE_ID;
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.Equal(DESTINATION_STAGE_ID, assignmnet.StageId);
    }
    [Fact]
    public async Task AssignmentController_MoveAssignmentToTheStage_DoestMoveAssignment_IfCalledByAnotherEmployee()
    {
        const string TOPIC = "Topic";
        const int DESTINATION_STAGE_ID = 2;
        await PrepareTestFixture();
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateStageAsync(new WorkflowStage
        {
            Id = DESTINATION_STAGE_ID,
            BoardId = 1,
            Name = "Second stage",
            Position = 2
        });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000));
        string RequestURI = "api/boards/1/tasks/1/move/" + DESTINATION_STAGE_ID;
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.NotNull(assignmnet);
        Assert.NotEqual(DESTINATION_STAGE_ID, assignmnet.StageId);
    }
    [Fact]
    public async Task AssignmentController_CompleteAssignmentById_CompletesAssignment_IfCalledByManager()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = false;
        await PrepareTestFixture();
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/complete";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.True(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_CompleteAssignmentById_CompletesAssignment_IfCalledByResponsibleEmployee()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = false;
        await PrepareTestFixture();
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/complete";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.True(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_CompleteAssignmentById_DoesNotCompleteAssignment_IfCalledByAnotherEmployee()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = false;
        await PrepareTestFixture();
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/complete";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.NotNull(assignmnet);
        Assert.False(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_UncompleteAssignmentById_UncompletesAssignment_IfCalledByManager()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = true;
        await PrepareTestFixture();
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/uncomplete";
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.False(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_UncompleteAssignmentById_UncompletesAssignment_IfCalledByResponsibleEmployee()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = true;
        await PrepareTestFixture();
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 100, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/uncomplete";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.NotNull(assignmnet);
        Assert.False(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_UncompleteAssignmentById_DoesNotUncompletesAssignment_IfCalledByAnotherEmployee()
    {
        const string TOPIC = "Topic";
        const bool ISCOMPLETED = true;
        await PrepareTestFixture();
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000, isCompleted: ISCOMPLETED));
        const string RequestURI = "api/boards/1/tasks/1/uncomplete";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.PutAsync(RequestURI, null);
        var assignmnet = await GetAssignmentFromTheDbByTopicAsync(TOPIC);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.NotNull(assignmnet);
        Assert.True(assignmnet.IsCompleted);
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_DeletesAssignment()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateEmployeeAsync(new Employee() { Id = 1000 });
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC, responsibleEmployeeId: 1000));
        const string RequestURI = "api/boards/1/tasks/1";
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
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_DeleteAssignmentById_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string TOPIC = "Test assignment 1";
        await _seedHelper.CreateAssignmentAsync(GetAssignment(topic: TOPIC));
        const string RequestURI = "api/boards/1/tasks/1";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }

    private async Task<bool> IsNumberOfAssignmentInTheDatabaseAsExpectedAsync(int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Assignments.CountAsync();
    }

    private async Task<Assignment?> GetAssignmentFromTheDbByTopicAsync(string topic)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Assignments.FirstOrDefaultAsync(a => a.Topic == topic);
    }

    private async Task<bool> DoesAssignmentWithSuchATopicExistInTheDatabaseAsync(string topic)
    {
        return await GetAssignmentFromTheDbByTopicAsync(topic) != null;
    }

    private static Assignment GetAssignment(int id = 1, string topic = "Topic", DateTime? deadline = null,
        int boardId = 1, int stageId = 1, int responsibleEmployeeId = 1, bool isCompleted = false)
    {
        return new Assignment()
        {
            Id = id,
            Topic = topic,
            Deadline = deadline ?? DateTime.MaxValue,
            BoardId = boardId,
            StageId = stageId,
            ResponsibleEmployeeId = responsibleEmployeeId,
            IsCompleted = isCompleted
        };
    }
}
