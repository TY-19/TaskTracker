using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class ChangePasswordModelValidatorTests
{
    private readonly ChangePasswordModelValidator _validator;
    public ChangePasswordModelValidatorTests()
    {
        _validator = new ChangePasswordModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "newPassword" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldBeInvalid_WhenOldPasswordIsNullOrEmpty(string? oldPassword)
    {
        var model = new ChangePasswordModel() { OldPassword = oldPassword!, NewPassword = "newPassword" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldBeInvalid_WhenNewPasswordIsNullOrEmpty(string? newPassword)
    {
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = newPassword! };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNewPasswordIsTooShort()
    {
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "123" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenOldAndNewPasswordAreEqual()
    {
        var model = new ChangePasswordModel() { OldPassword = "password", NewPassword = "password" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
