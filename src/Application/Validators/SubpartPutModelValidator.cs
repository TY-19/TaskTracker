using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class SubpartPutModelValidator : AbstractValidator<SubpartPutModel>
{
    public SubpartPutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Subpart name cannot be an empty string");
        RuleFor(x => x.Name).MaximumLength(50)
            .WithMessage("Subpart name length must be less than 50 characters");
    }
}
