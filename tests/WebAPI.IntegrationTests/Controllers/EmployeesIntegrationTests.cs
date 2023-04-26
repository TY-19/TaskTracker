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
}
