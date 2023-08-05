using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;

public class RegistrationRequestModelValidatorTests
{
    private readonly RegistrationRequestModelValidator _validator;
    public RegistrationRequestModelValidatorTests()
    {
        _validator = new RegistrationRequestModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new RegistrationRequestModel()
        {
            UserName = "NewUser",
            Email = "newuser@example.com",
            Password = "password"
        };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Theory]
    [InlineData("")]
    [InlineData("Aa")]
    [InlineData("VeryVeryVeryVeryVeryLongName")]
    [InlineData("With space")]
    [InlineData("Spec#Symbols")]
    [InlineData("User@#$%^&*")]
    [InlineData(null)]
    public void ShouldBeInvalid_WhenUserNameIsInvalid(string? username)
    {
        var model = new RegistrationRequestModel()
        {
            UserName = username!,
            Email = "newuser@example.com",
            Password = "password"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Theory]
    [InlineData("")]
    [InlineData("emailatexample.com")]
    [InlineData("email@example@com")]
    [InlineData(null)]
    public void ShouldBeInvalid_WhenEmailIsInvalid(string? email)
    {
        var model = new RegistrationRequestModel()
        {
            UserName = "NewUser",
            Email = email!,
            Password = "password"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("tootootootootootootoolongpassword")]
    [InlineData(null)]
    public void ShouldBeInvalid_WhenPasswordIsInvalid(string? password)
    {
        var model = new RegistrationRequestModel()
        {
            UserName = "NewUser",
            Email = "newuser@example.com",
            Password = password!
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}