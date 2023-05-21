using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class AssignmentPutModelValidator : AbstractValidator<AssignmentPutModel>
{
    public AssignmentPutModelValidator()
    {
        RuleFor(x => x.Topic).NotEqual(string.Empty)
            .WithMessage("Topic cannot be an empty string");
    }
}
