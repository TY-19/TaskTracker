using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class EmployeePutModelValidator : AbstractValidator<EmployeePutModel>
{
    public EmployeePutModelValidator()
    {
        RuleFor(x => x.FirstName).NotEqual(string.Empty)
            .WithMessage("First name cannot be an empty string");
        RuleFor(x => x.FirstName).MaximumLength(50)
            .WithMessage("First name length must be less than 50 characters");
        RuleFor(x => x.LastName).NotEqual(string.Empty)
            .WithMessage("Last name cannot be an empty string");
        RuleFor(x => x.LastName).MaximumLength(50)
            .WithMessage("Last name length must be less than 50 characters");
    }
}
