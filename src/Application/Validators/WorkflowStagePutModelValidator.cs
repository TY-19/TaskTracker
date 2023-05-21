using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class WorkflowStagePutModelValidator : AbstractValidator<WorkflowStagePutModel>
{
    public WorkflowStagePutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Name cannot be an empty string");
    }
}
