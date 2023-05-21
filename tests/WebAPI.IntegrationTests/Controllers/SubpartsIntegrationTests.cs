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

public class SubpartsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    private readonly DataSeedingHelper _seedHelper;

    public SubpartsIntegrationTests()
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
        await _seedHelper.CreateAssignmentAsync();
    }

    [Fact]
    public async Task AssignmentController_GetAllSubpartsOfTheAssignment_ReturnsCorrectNumbersOfSubparts()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = "First Part" });
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 2, AssignmentId = 1, Name = "Second Part" });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<SubpartGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact]
    public async Task AssignmentController_GetAllSubpartsOfTheAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = "First Part" });
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 2, AssignmentId = 1, Name = "Second Part" });
        const string RequestURI = $"api/boards/1/tasks/1/subparts";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsTheCorrectSubpart()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = SubpartName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<SubpartGetModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(SubpartName, result.Name);
    }
    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsNotFound_IfSubpartDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = SubpartName });
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_CreatesANewSubpart()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string SubpartName = "First Part";
        var subpart = new SubpartPostModel() { AssignmentId = 1, Name = SubpartName };
        var content = new StringContent(JsonSerializer.Serialize(subpart),
            Encoding.UTF8, "application/json");
        const string RequestURI = $"api/boards/1/tasks/1/subparts";

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await IsNumberOfSubpartsOfAssignmentAsExpectedAsync(1, 1));
        Assert.True(await DoesAssignmentContainsSubpartWithSuchANameAsync(1, SubpartName));
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsBadRequestStatusCode_IfAssignmentDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string SubpartName = "First Part";
        var subpart = new SubpartPostModel() { AssignmentId = 2, Name = SubpartName };
        var content = new StringContent(JsonSerializer.Serialize(subpart),
            Encoding.UTF8, "application/json");
        const string RequestURI = $"api/boards/1/tasks/2/subparts";

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await IsNumberOfSubpartsOfAssignmentAsExpectedAsync(2, 0));
        Assert.False(await DoesAssignmentContainsSubpartWithSuchANameAsync(2, SubpartName));
    }
    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var subpart = new SubpartPostModel();
        var content = new StringContent(JsonSerializer.Serialize(subpart),
            Encoding.UTF8, "application/json");
        const string RequestURI = $"api/boards/1/tasks/1/subparts";

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsBadRequestStatusCode_IfModelContainDifferentAssignmentId()
    {
        await PrepareTestFixture();
        const int IncorrectAssignmentId = 1;
        const int CorrectAssignmentId = 2;
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = CorrectAssignmentId, BoardId = 1, Topic = "Assignment 2" });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string SubpartName = "First Part";
        var subpart = new SubpartPostModel() { AssignmentId = CorrectAssignmentId, Name = SubpartName };
        var content = new StringContent(JsonSerializer.Serialize(subpart),
            Encoding.UTF8, "application/json");
        string RequestURI = $"api/boards/1/tasks/{IncorrectAssignmentId}/subparts";

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.Multiple(
            async () => Assert.True(await IsNumberOfSubpartsOfAssignmentAsExpectedAsync(1, 0)),
            async () => Assert.True(await IsNumberOfSubpartsOfAssignmentAsExpectedAsync(2, 0)),
            async () => Assert.False(await DoesAssignmentContainsSubpartWithSuchANameAsync(1, SubpartName)),
            async () => Assert.False(await DoesAssignmentContainsSubpartWithSuchANameAsync(2, SubpartName))
        );
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        var subpart = new SubpartPostModel() { AssignmentId = 1, Name = SubpartName };
        var content = new StringContent(JsonSerializer.Serialize(subpart),
            Encoding.UTF8, "application/json");
        const string RequestURI = $"api/boards/1/tasks/1/subparts";

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AssignmentController_UpdateSubpart_UpdatesSubpart()
    {
        await PrepareTestFixture();
        const string OldSubpartName = "Old Subpart Name";
        const string NewSubpartName = "New Subpart Name";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = OldSubpartName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";
        var model = new SubpartPutModel() { Name = NewSubpartName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesAssignmentContainsSubpartWithSuchANameAsync(1, OldSubpartName));
        Assert.True(await DoesAssignmentContainsSubpartWithSuchANameAsync(1, NewSubpartName));
    }
    [Fact]
    public async Task AssignmentController_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        const string OldSubpartName = "Old Subpart Name";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = OldSubpartName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";
        var model = new SubpartPutModel() { Name = string.Empty };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_UpdateSubpart_ReturnsBadRequestStatusCode_IfSubpartDoesNotExist()
    {
        await PrepareTestFixture();
        const string NewSubpartName = "New Subpart Name";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";
        var model = new SubpartPutModel() { Name = NewSubpartName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_UpdateSubpart_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string OldSubpartName = "Old Subpart Name";
        const string NewSubpartName = "New Subpart Name";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = OldSubpartName });
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";
        var model = new SubpartPutModel() { Name = NewSubpartName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_DeletesSubpart()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = SubpartName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesAssignmentContainsSubpartWithSuchANameAsync(1, SubpartName));
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_DoesNotDeleteSubpartIfItDoesNotBelongToThisAssignment()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        await _seedHelper.CreateAssignmentAsync(new Assignment() { Id = 2, BoardId = 1, Topic = "Assignment 2" });
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 2, Name = SubpartName });
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await DoesAssignmentContainsSubpartWithSuchANameAsync(2, SubpartName));
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string SubpartName = "First Part";
        await _seedHelper.CreateSubpartAsync(new Subpart() { Id = 1, AssignmentId = 1, Name = SubpartName });
        const string RequestURI = $"api/boards/1/tasks/1/subparts/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    private async Task<bool> IsNumberOfSubpartsOfAssignmentAsExpectedAsync(int assignmentId, int expected)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return expected == await context!.Subparts.Where(s => s.AssignmentId == assignmentId).CountAsync();
    }
    private async Task<bool> DoesAssignmentContainsSubpartWithSuchANameAsync(int assignmentId, string name)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Subparts.FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.Name == name) != null;
    }
}
