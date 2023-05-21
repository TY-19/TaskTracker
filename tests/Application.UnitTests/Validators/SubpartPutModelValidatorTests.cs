using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;

public class SubpartPutModelValidatorTests
{
    private readonly SubpartPutModelValidator _validator;
    public SubpartPutModelValidatorTests()
    {
        _validator = new SubpartPutModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new SubpartPutModel() { Name = "Updated" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsAnEmptyString()
    {
        var model = new SubpartPutModel() { Name = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
