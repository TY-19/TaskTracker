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
    [Fact]
    public void ShouldBeInvalid_WhenTopicIsTooLomg()
    {
        var model = new AssignmentPutModel()
        {
            Topic = "Very very very very very very very very very very very very very long topic",
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenDeadlineIsInThePast()
    {
        var dateTime = DateTime.Now.AddMinutes(-1);
        var model = new AssignmentPutModel()
        {
            Deadline = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Local),
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenProvidedWithInvalidSubpart()
    {
        var model = new AssignmentPutModel()
        {
            Subparts = new[]
            {
                new SubpartPutModel()
                {
                    Name = ""
                }
            }
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}