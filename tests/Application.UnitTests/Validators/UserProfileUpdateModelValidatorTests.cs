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
    [Fact]
    public void ShouldBeInvalid_WhenUserNameIsAnEmptyString()
    {
        var model = new UserProfileUpdateModel() { Email = "email@example.com", UserName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Theory]
    [InlineData("ab")]
    [InlineData("TheVeryVeryVeryVeryVeryLongUserName")]
    public void ShouldBeInvalid_WhenUserNameLengthIsInvalid(string userName)
    {
        var model = new UserProfileUpdateModel() { Email = "email@example.com", UserName = userName };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenEmailIsAnEmptyString()
    {
        var model = new UserProfileUpdateModel() { Email = string.Empty, UserName = "User" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenEmailIsInvalid()
    {
        var model = new UserProfileUpdateModel() { Email = "notEmail.com", UserName = "User" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
