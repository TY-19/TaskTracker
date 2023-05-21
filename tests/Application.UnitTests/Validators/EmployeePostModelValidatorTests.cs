using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class EmployeePostModelValidatorTests
{
    private readonly EmployeePostModelValidator _validator;
    public EmployeePostModelValidatorTests()
    {
        _validator = new EmployeePostModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new EmployeePostModel() { FirstName = "First", LastName = "Last" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenFirstNameIsNotProvided()
    {
        var model = new EmployeePostModel() { LastName = "Last" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenLastNameIsNotProvided()
    {
        var model = new EmployeePostModel() { FirstName = "First" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
