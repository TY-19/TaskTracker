using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class BoardPostModelValidatorTests
{
    private readonly BoardPostModelValidator _validator;
    public BoardPostModelValidatorTests()
    {
        _validator = new BoardPostModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new BoardPostModel() { Name = "Board name" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsNotProvided()
    {
        var model = new BoardPostModel();

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
