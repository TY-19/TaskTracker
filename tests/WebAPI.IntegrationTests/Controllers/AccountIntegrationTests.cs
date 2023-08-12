using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.Application.Models;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class AccountIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationTestsHelper _authHelper;
    public AccountIntegrationTests()
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
    public async Task AccountController_Login_ReturnsCorrectUserToken()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/login";
        var loginRequest = new LoginRequestModel() { NameOrEmail = "testemployee@example.com", Password = "Pa$$w0rd" };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<LoginResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Token);
    }
    [Fact]
    public async Task AccountController_Login_ReturnsNoUserToken_IfCredentialsAreInvalid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/login";
        var loginRequest = new LoginRequestModel() { NameOrEmail = "testemployee@example.com", Password = "Invalid" };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<LoginResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
    [Fact]
    public async Task AccountController_Login_ReturnsNoUserToken_IfRequestModelIsInvalid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/login";
        var loginRequest = new LoginRequestModel() { NameOrEmail = string.Empty, Password = string.Empty };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<LoginResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
    [Fact]
    public async Task AccountController_Registration_CreatesNewUser()
    {
        const string RequestURI = "api/account/registration";
        const string UserName = "NewTestUser";
        var registrationRequest = new RegistrationRequestModel()
        {
            UserName = UserName,
            Email = "newtestuser@example.com",
            Password = "Pa$$w0rd"
        };
        var content = new StringContent(JsonSerializer.Serialize(registrationRequest),
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
    public async Task AccountController_Registration_ReturnsUnsuccessResult_IfModelIsInvalid()
    {
        const string RequestURI = "api/account/registration";
        const string UserName = "NewTestUser";
        var registrationRequest = new RegistrationRequestModel()
        {
            UserName = UserName,
            Email = "NOTEmail",
            Password = "Pa$$w0rd"
        };
        var content = new StringContent(JsonSerializer.Serialize(registrationRequest),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<RegistrationResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.False(await DoesUserWithSuchANameExistInTheDatabaseAsync(UserName));
    }
    [Fact]
    public async Task AccountController_Registration_ReturnsUnsuccessResult_IfUserAlreadyExist()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/registration";
        const string UserName = "testemployee";
        var registrationRequest = new RegistrationRequestModel()
        {
            UserName = UserName,
            Email = "testemployee@example.com",
            Password = "Pa$$w0rd"
        };
        var content = new StringContent(JsonSerializer.Serialize(registrationRequest),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<RegistrationResponseModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.False(result.Success);
    }
    [Fact]
    public async Task AccountController_ViewProfile_ReturnsUserProfile()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var httpResponse = await _httpClient.GetAsync(RequestURI);
        httpResponse.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<UserProfileModel>(httpResponse.Content.ReadAsStream(),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.NotNull(result);
        Assert.Equal("testemployee", result.UserName);
    }
    [Fact]
    public async Task AccountController_ViewProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile";

        var httpResponse = await _httpClient.GetAsync(RequestURI);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AccountController_UpdateProfile_UpdatesUserProfile()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string NewEmail = "newemail@example.com";
        var model = new UserProfileUpdateModel() { Email = NewEmail };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await DoesUserWithSuchAnEmailExistInTheDatabaseAsync(NewEmail));
        Assert.False(await DoesUserWithSuchAnEmailExistInTheDatabaseAsync("testemployee@example.com"));
    }
    [Fact]
    public async Task AccountController_UpdateProfile_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new UserProfileUpdateModel() { Email = "notEmail.com" };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AccountController_UpdateProfile_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile";
        const string NewEmail = "newemail@example.com";
        var model = new UserProfileUpdateModel() { Email = NewEmail };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    [Fact]
    public async Task AccountController_ChangePassword_ChangesUserPassword()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile/changepassword";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string NewPassword = "NewPassword";
        var model = new ChangePasswordModel() { OldPassword = "Pa$$w0rd", NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);
        httpResponse.EnsureSuccessStatusCode();

        Assert.True(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", NewPassword));
        Assert.False(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", "Pa$$w0rd"));
    }
    [Fact]
    public async Task AccountController_ChangePassword_DoesNotChangePassword_IfOldPasswordIsInvalid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile/changepassword";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string NewPassword = "NewPassword";
        var model = new ChangePasswordModel() { OldPassword = "Invalid", NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
        Assert.True(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", "Pa$$w0rd"));
        Assert.False(await AuthenticationTestsHelper.IsTryLoginSuccessfulAync(_httpClient, "testemployee", NewPassword));
    }
    [Fact]
    public async Task AccountController_ChangePassword_ReturnsBadRequest_IfOldPasswordIsInvalid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile/changepassword";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const string NewPassword = "NewPassword";
        var model = new ChangePasswordModel() { OldPassword = "Invalid", NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AccountController_ChangePassword_ReturnsBadRequestStatusCode_IfModelIsNotValid()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile/changepassword";
        string? token = _authHelper.TestEmployeeUserToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var model = new ChangePasswordModel() { OldPassword = "Pa$$w0rd", NewPassword = "123" };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
    }
    [Fact]
    public async Task AccountController_ChangePassword_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        await PrepareTestFixture();
        const string RequestURI = "api/account/profile/changepassword";
        const string NewPassword = "NewPassword";
        var model = new ChangePasswordModel() { OldPassword = "Pa$$w0rd", NewPassword = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(model),
            Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(RequestURI, content);

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)httpResponse.StatusCode);
    }

    private async Task<bool> DoesUserWithSuchANameExistInTheDatabaseAsync(string userName)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Users.FirstOrDefaultAsync(a => a.UserName == userName) != null;
    }
    private async Task<bool> DoesUserWithSuchAnEmailExistInTheDatabaseAsync(string email)
    {
        using var test = _factory.Services.CreateScope();
        var context = test.ServiceProvider.GetService<TrackerDbContext>();
        return await context!.Users.FirstOrDefaultAsync(a => a.Email == email) != null;
    }
}
