namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class UsersIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public UsersIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
}
