using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class SubpartPutModelValidator : AbstractValidator<SubpartPutModel>
{
    public SubpartPutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Name cannot be an empty string");
    }
}
