using FluentValidation;
using FluentValidation.Results;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.Services;

public class ValidationService : IValidationService
{
    private readonly Dictionary<Type, IValidator> ModelsValidators = new()
    {
        { typeof(AssignmentPostModel), new AssignmentPostModelValidator() },
        { typeof(AssignmentPutModel), new AssignmentPutModelValidator() },
        { typeof(BoardPostModel), new BoardPostModelValidator() },
        { typeof(BoardPutModel), new BoardPutModelValidator() },
        { typeof(ChangePasswordModel), new ChangePasswordModelValidator() },
        { typeof(EmployeePostModel), new EmployeePostModelValidator() },
        { typeof(EmployeePutModel), new EmployeePutModelValidator() },
        { typeof(LoginRequestModel), new LoginRequestModelValidator() },
        { typeof(RegistrationRequestModel), new RegistrationRequestModelValidator() },
        { typeof(SetPasswordModel), new SetPasswordModelValidator() },
        { typeof(SubpartPostModel), new SubpartPostModelValidator() },
        { typeof(SubpartPutModel), new SubpartPutModelValidator() },
        { typeof(UserProfileUpdateModel), new UserProfileUpdateModelValidator() },
        { typeof(WorkflowStagePostModel), new WorkflowStagePostModelValidator() },
        { typeof(WorkflowStagePutModel), new WorkflowStagePutModelValidator() },
    };

    public ValidationResult Validate(IValidatableModel model)
    {
        var validator = ModelsValidators[model.GetType()];
        return ((dynamic)validator).Validate((dynamic)model);
    }
}
