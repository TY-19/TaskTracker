using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class EmployeePostModelValidator : AbstractValidator<EmployeePostModel>
{
    public EmployeePostModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("First name is required");
        RuleFor(x => x.FirstName).MaximumLength(50)
            .WithMessage("First name length must be less than 50 characters");
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Last name is required");
        RuleFor(x => x.LastName).MaximumLength(50)
            .WithMessage("Last name length must be less than 50 characters");
    }
}
