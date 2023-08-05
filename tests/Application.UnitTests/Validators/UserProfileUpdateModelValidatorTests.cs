using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class UserProfileUpdateModelValidatorTests
{
    private readonly UserProfileUpdateModelValidator _validator;
    public UserProfileUpdateModelValidatorTests()
    {
        _validator = new UserProfileUpdateModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new UserProfileUpdateModel() { Email = "email@example.com", UserName = "User" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("TheVeryVeryVeryVeryVeryLongUserName")]
    [InlineData("With space")]
    [InlineData("Spec#Symbols")]
    [InlineData("User@#$%^&*")]
    public void ShouldBeInvalid_WhenUserNameIsInvalid(string userName)
    {
        var model = new UserProfileUpdateModel() { Email = "email@example.com", UserName = userName };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("emailatexample.com")]
    [InlineData("email@example@com")]
    public void ShouldBeInvalid_WhenEmailIsInvalid(string email)
    {
        var model = new UserProfileUpdateModel() { Email = email, UserName = "User" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldBeInvalid_WhenFirstNameIsAnEmptyString()
    {
        var model = new UserProfileUpdateModel() { FirstName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenLastNameIsAnEmptyString()
    {
        var model = new UserProfileUpdateModel() { LastName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenFirstNameIsTooLomg()
    {
        var model = new UserProfileUpdateModel()
        {
            FirstName = "Very very very very very very very very very long first name",
            LastName = "Last"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenLastNameIsTooLong()
    {
        var model = new UserProfileUpdateModel()
        {
            FirstName = "First",
            LastName = "Very very very very very very very very very long last name"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
