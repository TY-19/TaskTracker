using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class WorkflowStagePostModelValidator : AbstractValidator<WorkflowStagePostModel>
{
    public WorkflowStagePostModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required");
    }
}
