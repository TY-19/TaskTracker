namespace TaskTracker.Application.UnitTests.Services;

public class AccountServiceTests
{
    [Fact]
    public async Task LoginAsync_Success_WhenProvidedWithCorrectCredentials()
    { 
        throw new NotImplementedException();
    }
    [Fact]
    public async Task LoginAsync_ReturnsResponseThatContainsAToken_WhenProvidedWithCorrectCredentials()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task LoginAsync_ReturnResponseWithFalse_WhenProvidedWithIncorrectCredentials()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task LoginAsync_ReturnResponseWithoutAToken_WhenProvidedWithIncorrectCredentials()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task RegistrationAsync_ReturnsResponsWithTrue_IfProvidedValidData()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task RegistrationAsync_ReturnsResponsWithFalse_IfProvidedInvalidData()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task RegistrationAsync_ReturnsResponsWithFalse_IfUserWithSuchANameAlreadyExists()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsUserProfile_IfUserExists()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsNull_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfileWithCorrectEmployee_IfUserIsAnEmpoloyee()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfileWithoutEmployee_IfUserIsNotAnEmpoloyee()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsTrueAndUpdatesUserProfile()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsTrueAndUpdatesEmployee()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsFalse_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsTrueAndChangesPassword_IfProvidedWithValidData()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalseAndDoesNotChangePassword_IfUserDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalseAndDoesNotChangePassword_IfProvidedPreviousPasswordIsNotCorrect()
    {
        throw new NotImplementedException();
    }
}
