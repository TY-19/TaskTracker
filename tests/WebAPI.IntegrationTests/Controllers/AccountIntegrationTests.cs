namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class AccountIntegrationTests
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _httpClient;
	public AccountIntegrationTests()
	{
		_factory = new CustomWebApplicationFactory();
		_httpClient = _factory.CreateClient();
	}

}
