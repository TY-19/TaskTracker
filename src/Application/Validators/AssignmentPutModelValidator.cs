using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class AssignmentPutModelValidator : AbstractValidator<AssignmentPutModel>
{
    public AssignmentPutModelValidator()
    {
        RuleFor(x => x.Topic).NotEqual(string.Empty)
            .WithMessage("Topic cannot be an empty string");
        RuleFor(x => x.Topic).MaximumLength(50)
            .WithMessage("Topic length must be less than 50 characters");
        RuleFor(x => x.Deadline).GreaterThan(_ => DateTime.Now)
            .WithMessage("Deadline has to be in the future");
        RuleForEach(x => x.Subparts)
            .SetValidator(new SubpartPutModelValidator());
    }
}
