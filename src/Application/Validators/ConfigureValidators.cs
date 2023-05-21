using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public static class ConfigureValidators
{
    private static readonly Dictionary<Type, IValidator> ModelsValidators = new()
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
    public static IValidator GetValidatorFor(Type type)
    {
        return ModelsValidators[type];
    }
}
