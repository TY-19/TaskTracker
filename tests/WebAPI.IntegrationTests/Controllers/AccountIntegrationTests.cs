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

	[Fact]
	public async Task AccountController_Login_ReturnsCorrectUserToken()
	{
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_Login_ReturnsNoUserToken_IfCredentialsAreInvalid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_Registration_CreatesNewUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_Registration_ReturnsUnsuccessResult_IfModelIsInvalid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_Registration_ReturnsUnsuccessResult_IfUserAlreadyExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ViewProfile_ReturnsUserProfile()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ViewProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ViewProfile_ReturnsNotFoundStatusCode_IfUserDoesNotExist ()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_UpdateProfile_UpdatesUserProfile()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_UpdateProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_UpdateProfile_ReturnsBadRequestStatusCode_IfUserToUpdatesIsNotTheSameUserThatCalledIt()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ChangePassword_ChangesUserPassword()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ChangePassword_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AccountController_ChangePassword_ReturnsBadRequestStatusCode_IfChangesPasswordNotForCurrentUser()
    {
        throw new NotImplementedException();
    }
}
