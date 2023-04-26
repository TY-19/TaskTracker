namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class BoardsIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    public BoardsIntegrationTests()
	{
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }
}
