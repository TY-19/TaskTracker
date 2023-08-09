using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class SubpartPostModelValidatorTests
{
    private readonly SubpartPostModelValidator _validator;
    public SubpartPostModelValidatorTests()
    {
        _validator = new SubpartPostModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new SubpartPostModel() { Name = "Subpart", AssignmentId = 1 };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsNotProvided()
    {
        var model = new SubpartPostModel() { AssignmentId = 1 };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsTooLong()
    {
        var model = new SubpartPostModel()
        {
            Name = "Very very very very very very very very very very very very very long name",
            AssignmentId = 1
        };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
