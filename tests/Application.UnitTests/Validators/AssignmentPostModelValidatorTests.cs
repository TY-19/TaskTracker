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
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ShouldBeInvalid_WhenTopicIsNotProvided()
    {
        var model = new AssignmentPostModel()
        {
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenTopicIsTooLomg()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "Very very very very very very very very very very very very very long topic",
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenStageIdIsNotProvided()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = DateTime.MaxValue,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenResponsibleEmployeeIdIsNotProvided()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = DateTime.MaxValue,
            StageId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenDeadlineIsNotProvided()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenDeadlineIsInThePast()
    {
        var dateTime = DateTime.Now.AddMinutes(-1);
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Local),
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenProvidedWithInvalidSubpart()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1,
            Subparts = new[]
            {
                new SubpartPostModel()
                {
                    Name = ""
                }
            }
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
