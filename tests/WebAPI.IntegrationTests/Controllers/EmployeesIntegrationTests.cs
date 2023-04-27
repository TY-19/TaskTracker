namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class EmployeesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public EmployeesIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployeesOfTheBoard_ReturnsCorrectNumbersOfEmployees()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_GetAllEmployeesOfTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsTheCorrectEmployee()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsNotFoundStatusCode_IfEmployeeDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_GetEmployeeById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_AddsEmployeeToTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsBadRequestStatusCode_IfEmployeeDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsBadRequestStatusCode_IfBoardDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_AddEmployeeToTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_RemovesEmployeeFromTheBoard()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task EmployeesController_RemoveEmployeeFromTheBoard_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
}
