using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class AssignmentPostModelValidator : AbstractValidator<AssignmentPostModel>
{
    public AssignmentPostModelValidator()
    {
        RuleFor(x => x.Topic).NotEmpty()
            .WithMessage("Topic is required");
        RuleFor(x => x.StageId).NotEmpty()
            .WithMessage("Id of the stage must be provided");
    }
}
