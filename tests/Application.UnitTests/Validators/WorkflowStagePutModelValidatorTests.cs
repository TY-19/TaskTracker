using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class WorkflowStagePutModelValidatorTests
{
    private readonly WorkflowStagePutModelValidator _validator;
    public WorkflowStagePutModelValidatorTests()
    {
        _validator = new WorkflowStagePutModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new WorkflowStagePutModel() { Name = "Updated" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsAnEmptyString()
    {
        var model = new WorkflowStagePutModel() { Name = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
