using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class AssignmentPostModelValidator : AbstractValidator<AssignmentPostModel>
{
    public AssignmentPostModelValidator()
    {
        RuleFor(x => x.Topic).NotEmpty()
            .WithMessage("Topic is required");
        RuleFor(x => x.Topic).MaximumLength(50)
            .WithMessage("Topic length must be less than 50 characters");
        RuleFor(x => x.StageId).NotEmpty()
            .WithMessage("Id of the stage must be provided");
        RuleFor(x => x.Deadline).NotEmpty().GreaterThan(_ => DateTime.Now)
            .WithMessage("Deadline has to be in the future");
        RuleFor(x => x.ResponsibleEmployeeId).NotEmpty()
            .WithMessage("Id of the responsible employee must be provided");
        RuleForEach(x => x.Subparts)
            .SetValidator(new SubpartPostModelValidator());
    }
}
