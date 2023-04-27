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
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsCorrectNumbersOfUsers()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsTheCorrectUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsNotFoundStatusCode_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_CreatesUserProfile()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_DoesNotCreateUserProfile_IfUserAlreadyExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsUnsuccessResult_IfModelIsInvalid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_UpdatesUserProfile()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsBadRequest_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsBadRequest_IfModelIsInvalid()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ChangesUserPassword()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsBadRequest_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_DeleteUser_DeletesUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        throw new NotImplementedException();
    }
}
