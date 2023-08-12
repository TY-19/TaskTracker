using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class UsersIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    public UsersIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
        _authHelper = new AuthenticationTestsHelper(_factory);
    }
    private async Task PrepareTestFixture()
    {
        await _authHelper.ConfigureAuthenticatorAsync();
    }
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsCorrectNumbersOfUsers()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<UserProfileModel>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(4, result.Count());
    }
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_GetAllUsers_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsTheCorrectUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<UserProfileModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("testemployee", result.UserName);
        Assert.Equal("testemployee@example.com", result.Email);
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsNotFoundStatusCode_IfUserDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/nonexistinguser";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/testemployee";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_GetUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_CreatesUserProfile()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";
        const string UserName = "NewUser";
        var user = new RegistrationRequestModel() { UserName = UserName, Email = "newuser@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<RegistrationResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_DoesNotCreateUserProfile_IfUserAlreadyExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";
        var user = new RegistrationRequestModel() { UserName = "testadmin", Email = "testadmin@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<RegistrationResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsUnsuccessResult_IfModelIsInvalid()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";
        var user = new RegistrationRequestModel() { UserName = "NewName", Email = "notemail", Password = "1234" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<RegistrationResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/";
        const string UserName = "NewUser";
        var user = new RegistrationRequestModel() { UserName = UserName, Email = "newuser@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";
        const string UserName = "NewUser";
        var user = new RegistrationRequestModel() { UserName = UserName, Email = "newuser@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_CreateUserProfile_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/";
        const string UserName = "NewUser";
        var user = new RegistrationRequestModel() { UserName = UserName, Email = "newuser@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(user),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_UpdatesUserProfile()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";
        const string UserName = "UpdatedName";
        var model = new UserProfileUpdateModel()
        {
            UserName = UserName,
            Roles = new List<string>() {
            DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE }
        };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
        Assert.False(await DoesUserWithSuchANameExistInTheDatabaseAsync("testemployee"));
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsBadRequest_IfUserDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/nonexistinguser";
        const string UserName = "UpdatedName";
        var model = new UserProfileUpdateModel() { UserName = UserName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsBadRequest_IfModelIsInvalid()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";
        var model = new UserProfileUpdateModel() { Email = "notemail" };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/testemployee";
        const string UserName = "UpdatedName";
        var model = new UserProfileUpdateModel() { UserName = UserName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";
        const string UserName = "UpdatedName";
        var model = new UserProfileUpdateModel() { UserName = UserName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_UpdateUserProfile_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee";
        const string UserName = "UpdatedName";
        var model = new UserProfileUpdateModel() { UserName = UserName };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ChangesUserPassword()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee/changepassword";
        const string NewPassword = "$tr0ngNewPassword";
        var model = new SetPasswordModel() { NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", NewPassword));
        Assert.False(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", "Pa$$w0rd"));
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsBadRequestStatusCode_IfModelIsNotValidd()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee/changepassword";
        var model = new SetPasswordModel() { NewPassword = string.Empty };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsBadRequest_IfUserDoesNotExist()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/nonexistinguser/changepassword";
        const string NewPassword = "$tr0ngNewPassword";
        var model = new SetPasswordModel() { NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/testemployee/changepassword";
        const string NewPassword = "$tr0ngNewPassword";
        var model = new SetPasswordModel() { NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee/changepassword";
        const string NewPassword = "$tr0ngNewPassword";
        var model = new SetPasswordModel() { NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_ChangeUserPassword_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/testemployee/changepassword";
        const string NewPassword = "$tr0ngNewPassword";
        var model = new SetPasswordModel() { NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_DeleteUser_DeletesUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string UserName = "testemployee";
        string RequestURI = $"api/users/{UserName}";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        Assert.False(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string UserName = "testemployee";
        string RequestURI = $"api/users/{UserName}";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
        Assert.True(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string UserName = "testemployee";
        string RequestURI = $"api/users/{UserName}";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.True(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task UsersController_DeleteUser_ReturnsForbiddenStatusCode_IfCalledByManagerUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestManagerUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string UserName = "testemployee";
        string RequestURI = $"api/users/{UserName}";

        var httpResponse = await _httpClient.DeleteAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
        Assert.True(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task UsersController_GetAllRoles_ReturnsCorrectNumbersOfRoles()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestAdminUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/roles";

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<IEnumerable<string>>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }
    [Fact]
    public async Task UsersController_GetAllRoles_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/users/roles";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task UsersController_GetAllRoles_ReturnsForbiddenStatusCode_IfCalledByEmployeeUser()
    {
        await PrepareTestFixture();
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string RequestURI = "api/users/roles";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status403Forbidden, (int)httpResponse.StatusCode);
    }

    private async Task<bool> DoesUserWithSuchANameExistInTheDatabaseAsync(string userName)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Users.FirstOrDefaultAsync(a => a.UserName == userName) != null;
    }
}
