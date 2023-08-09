using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class WorkflowStagePostModelValidatorTests
{
    private readonly WorkflowStagePostModelValidator _validator;
    public WorkflowStagePostModelValidatorTests()
    {
        _validator = new WorkflowStagePostModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new WorkflowStagePostModel() { Name = "Stage 1" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsNotProvided()
    {
        var model = new WorkflowStagePostModel();

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsEmpty()
    {
        var model = new WorkflowStagePostModel() { Name = "" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsTooLong()
    {
        var model = new WorkflowStagePostModel()
        {
            Name = "Very very very very very very very very very very very very very long name"
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
