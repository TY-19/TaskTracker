using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;

namespace TaskTracker.Application.UnitTests.Services;

public class ValidationServiceTests
{
    private readonly IValidationService _validationService;
    public ValidationServiceTests()
    {
        _validationService = new ValidationService();
    }

    [Fact]
    public void Validate_ReturnsValidResult_IfAssignmentPostModelIsValid()
    {
        var model = new AssignmentPostModel()
        {
            Topic = "New topic",
            Deadline = DateTime.MaxValue,
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfAssignmentPostModelIsInvalid()
    {
        var model = new AssignmentPostModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfAssignmentPutModelIsValid()
    {
        var model = new AssignmentPutModel() { Topic = "Updated" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfAssignmentPutModelIsInvalid()
    {
        var model = new AssignmentPutModel() { Topic = string.Empty };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfBoardPostModelIsValid()
    {
        var model = new BoardPostModel() { Name = "Board name" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfBoardPostModelIsInvalid()
    {
        var model = new BoardPostModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfBoardPutModelIsValid()
    {
        var model = new BoardPutModel() { Name = "Updated" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfBoardPutModelIsInvalid()
    {
        var model = new BoardPutModel() { Name = string.Empty };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfChangePasswordModelIsValid()
    {
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "newPassword" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfChangePasswordModel()
    {
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "123" };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfEmployeePostModelIsValid()
    {
        var model = new EmployeePostModel() { FirstName = "First", LastName = "Last" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfEmployeePostModelIsInvalid()
    {
        var model = new EmployeePostModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfEmployeePutModelIsValid()
    {
        var model = new EmployeePutModel() { FirstName = "Updated" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfEmployeePutModelIsInvalid()
    {
        var model = new EmployeePutModel() { FirstName = string.Empty, LastName = string.Empty };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfLoginRequestModelIsValid()
    {
        var model = new LoginRequestModel() { NameOrEmail = "Test", Password = "12345678" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfLoginRequestModelIsInvalid()
    {
        var model = new LoginRequestModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfRegistrationRequestModelIsValid()
    {
        var model = new RegistrationRequestModel()
        {
            UserName = "user",
            Email = "email@example.com",
            Password = "Pa$$w0rd"
        };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfRegistrationRequestModelIsInvalid()
    {
        var model = new RegistrationRequestModel()
        {
            UserName = "user",
            Email = "notemail",
        };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfSetPasswordModelIsValid()
    {
        var model = new SetPasswordModel() { NewPassword = "password" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfSetPasswordModelIsInvalid()
    {
        var model = new SetPasswordModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfSubpartPostModelIsValid()
    {
        var model = new SubpartPostModel() { Name = "Subpart", AssignmentId = 1 };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfSubpartPostModelIsInvalid()
    {
        var model = new SubpartPostModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfSubpartPutModelIsValid()
    {
        var model = new SubpartPutModel() { Name = "Updated" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfSubpartPutModelIsInvalid()
    {
        var model = new SubpartPutModel() { Name = string.Empty };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfUserProfileUpdateModelIsValid()
    {
        var model = new UserProfileUpdateModel() { Email = "email@example.com", UserName = "User" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfUserProfileUpdateModelIsInvalid()
    {
        var model = new UserProfileUpdateModel() { Email = "notEmail.com" };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfWorkflowStagePostModelIsValid()
    {
        var model = new WorkflowStagePostModel() { Name = "Stage 1" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfWorkflowStagePostModelIsInvalid()
    {
        var model = new WorkflowStagePostModel();

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsValidResult_IfWorkflowStagePutModelIsValid()
    {
        var model = new WorkflowStagePutModel() { Name = "Updated" };

        var result = _validationService.Validate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void Validate_ReturnsInvalidResult_IfWorkflowStagePutModelIsInvalid()
    {
        var model = new WorkflowStagePutModel() { Name = string.Empty };

        var result = _validationService.Validate(model);

        Assert.False(result.IsValid);
    }
}
