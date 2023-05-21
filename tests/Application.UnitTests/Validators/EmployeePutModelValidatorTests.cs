using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class EmployeePutModelValidatorTests
{
    private readonly EmployeePutModelValidator _validator;
    public EmployeePutModelValidatorTests()
    {
        _validator = new EmployeePutModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new EmployeePutModel() { FirstName = "Updated" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenFirstNameIsAnEmptyString()
    {
        var model = new EmployeePutModel() { FirstName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenLastNameIsAnEmptyString()
    {
        var model = new EmployeePutModel() { LastName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
