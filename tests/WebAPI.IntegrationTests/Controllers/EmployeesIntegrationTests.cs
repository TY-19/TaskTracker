using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class EmployeesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    private readonly DataSeedingHelper _seedHelper;
    public EmployeesIntegrationTests()
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
    public async Task EmployeesController_GetAllEmployees_ReturnsCorrectNumbersOfEmployees()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var board1 = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        var board2 = new Board() { Id = 2, Employees = new List<Employee>() { employee2 } };
        await _seedHelper.CreateBoardAsync(board1);
        await _seedHelper.CreateBoardAsync(board2);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<EmployeeGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployees_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var employee3 = new Employee() { Id = 3, FirstName = "FirstName3", LastName = "LastName3" };
        var board1 = new Board() { Id = 1, Employees = new List<Employee>() { employee1, employee2 } };
        var board2 = new Board() { Id = 2, Employees = new List<Employee>() { employee3 } };
        await _seedHelper.CreateBoardAsync(board1);
        await _seedHelper.CreateBoardAsync(board2);
        const string RequestURI = "api/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployees_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var employee3 = new Employee() { Id = 3, FirstName = "FirstName3", LastName = "LastName3" };
        var board1 = new Board() { Id = 1, Employees = new List<Employee>() { employee1, employee2 } };
        var board2 = new Board() { Id = 2, Employees = new List<Employee>() { employee3 } };
        await _seedHelper.CreateBoardAsync(board1);
        await _seedHelper.CreateBoardAsync(board2);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployeesOfTheBoard_ReturnsCorrectNumbersOfEmployees()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1, employee2 } };
        await _seedHelper.CreateBoardAsync(board);
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<EmployeeGetModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployeesOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1, employee2 } };
        await _seedHelper.CreateBoardAsync(board);
        const string RequestURI = "api/boards/1/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployeesOfTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeThatIsNotPartOfBoard()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName1", LastName = "LastName1" };
        var employee2 = new Employee() { Id = 2, FirstName = "FirstName2", LastName = "LastName2" };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1, employee2 } };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsTheCorrectEmployee()
    {
        await PrepareTestFixture();
        const string FirstName = "FirstName";
        const string LastName = "LastName";
        var employee1 = new Employee() { Id = 1, FirstName = FirstName, LastName = LastName };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        await _seedHelper.CreateBoardAsync(board);
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<EmployeeGetModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(FirstName, result.FirstName);
        Assert.Equal(LastName, result.LastName);
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsNotFoundStatusCode_IfEmployeeDoesNotExist()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1 };
        await _seedHelper.CreateBoardAsync(board);
        await _seedHelper.AddEmployeeToTheBoardAsync(100, 1);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string FirstName = "FirstName";
        const string LastName = "LastName";
        var employee = new Employee() { Id = 1, FirstName = FirstName, LastName = LastName };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee } };
        await _seedHelper.CreateBoardAsync(board);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsForbiddenStatusCode_IfCalledByEmployeeThatIsNotPartOfBoard()
    {
        await PrepareTestFixture();
        var employee1 = new Employee() { Id = 1, FirstName = "FirstName", LastName = "LastName" };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_AddsEmployeeToTheBoard()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Employees = new List<Employee>() };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/testemployee";

        var httpResponse = await _httpClient.PostAsync(RequestURI, null);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await DoesEmployeeExistInTheBoardAsync(1, "Test"));
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsBadRequestStatusCode_IfEmployeeDoesNotExist()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Employees = new List<Employee>() };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/nonexistingemployee";

        var httpResponse = await _httpClient.PostAsync(RequestURI, null);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Employees = new List<Employee>() };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/999/employees/testemployee";

        var httpResponse = await _httpClient.PostAsync(RequestURI, null);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Employees = new List<Employee>() };
        await _seedHelper.CreateBoardAsync(board);
        const string RequestURI = "api/boards/1/employees/testemployee";

        var httpResponse = await _httpClient.PostAsync(RequestURI, null);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        var board = new Board() { Id = 1, Employees = new List<Employee>() };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/testemployee";

        var httpResponse = await _httpClient.PostAsync(RequestURI, null);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_RemovesEmployeeFromTheBoard()
    {
        await PrepareTestFixture();
        const string FirstName = "FirstName";
        const string LastName = "LastName";
        var employee1 = new Employee() { Id = 1, FirstName = FirstName, LastName = LastName };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesEmployeeExistInTheBoardAsync(1, FirstName));
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string FirstName = "FirstName";
        const string LastName = "LastName";
        var employee1 = new Employee() { Id = 1, FirstName = FirstName, LastName = LastName };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        await _seedHelper.CreateBoardAsync(board);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        const string FirstName = "FirstName";
        const string LastName = "LastName";
        var employee1 = new Employee() { Id = 1, FirstName = FirstName, LastName = LastName };
        var board = new Board() { Id = 1, Employees = new List<Employee>() { employee1 } };
        await _seedHelper.CreateBoardAsync(board);
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/boards/1/employees/1";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    private async Task<bool> DoesEmployeeExistInTheBoardAsync(int boardId, string employeeFirstName)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        var employee = await context!.Employees
            .Include(e => e.Boards)
            .FirstOrDefaultAsync(e => e.FirstName == employeeFirstName);
        return employee?.Boards.Any(b => b.Id == boardId) ?? false;
    }
}
