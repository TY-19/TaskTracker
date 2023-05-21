using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;

public class AssignmentPutModelValidatorTests
{
    private readonly AssignmentPutModelValidator _validator;
    public AssignmentPutModelValidatorTests()
    {
        _validator = new AssignmentPutModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new AssignmentPutModel() { Topic = "Updated" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNewTopicIsAnEmptyString()
    {
        var model = new AssignmentPutModel() { Topic = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
