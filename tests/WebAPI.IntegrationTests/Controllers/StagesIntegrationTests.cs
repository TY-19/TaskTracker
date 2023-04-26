namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class StagesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public StagesIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
}
