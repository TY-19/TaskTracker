using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class SetPasswordModelValidatorTests
{
    private readonly SetPasswordModelValidator _validator;
    public SetPasswordModelValidatorTests()
    {
        _validator = new SetPasswordModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new SetPasswordModel() { NewPassword = "password" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenPasswordIsAnEmptyString()
    {
        var model = new SetPasswordModel() { NewPassword = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
