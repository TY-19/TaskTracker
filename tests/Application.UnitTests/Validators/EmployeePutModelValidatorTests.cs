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
        var model = new EmployeePutModel() { FirstName = string.Empty, LastName = "Last" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenLastNameIsAnEmptyString()
    {
        var model = new EmployeePutModel() { FirstName = "First", LastName = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenFirstNameIsTooLomg()
    {
        var model = new EmployeePutModel()
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
        var model = new EmployeePutModel()
        {
            FirstName = "First",
            LastName = "Very very very very very very very very very long last name"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
