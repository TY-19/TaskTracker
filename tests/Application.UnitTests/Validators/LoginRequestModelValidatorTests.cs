using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class LoginRequestModelValidatorTests
{
    private readonly LoginRequestModelValidator _validator;
    public LoginRequestModelValidatorTests()
    {
        _validator = new LoginRequestModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new LoginRequestModel() { NameOrEmail = "Test", Password = "12345678" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameOrEmailIsNotProvided()
    {
        var model = new LoginRequestModel() { Password = "12345678" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenPasswordIsNotProvided()
    {
        var model = new LoginRequestModel() { NameOrEmail = "Test" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
