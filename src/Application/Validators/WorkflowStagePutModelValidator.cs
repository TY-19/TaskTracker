using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class WorkflowStagePutModelValidator : AbstractValidator<WorkflowStagePutModel>
{
    public WorkflowStagePutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Name cannot be an empty string");
        RuleFor(x => x.Name).MaximumLength(50)
            .WithMessage("Name length must be less than 50 characters");
    }
}
