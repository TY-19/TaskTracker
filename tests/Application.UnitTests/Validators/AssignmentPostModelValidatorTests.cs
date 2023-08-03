using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;

public class AssignmentPostModelValidatorTests
{
    private readonly AssignmentPostModelValidator _validator;
    public AssignmentPostModelValidatorTests()
    {
        _validator = new AssignmentPostModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new AssignmentPostModel() { Topic = "New topic",
            Deadline = DateTime.MaxValue, StageId = 1, ResponsibleEmployeeId = 1 };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ShouldBeInvalid_WhenTopicIsNotProvided()
    {
        var model = new AssignmentPostModel() { StageId = 1 };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenStageIdIsNotProvided()
    {
        var model = new AssignmentPostModel() { Topic = "New topic" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
