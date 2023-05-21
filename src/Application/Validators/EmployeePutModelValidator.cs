using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class EmployeePutModelValidator : AbstractValidator<EmployeePutModel>
{
    public EmployeePutModelValidator()
    {
        RuleFor(x => x.FirstName).NotEqual(string.Empty)
            .WithMessage("First name cannot be an empty string");
        RuleFor(x => x.LastName).NotEqual(string.Empty)
            .WithMessage("Last name cannot be an empty string");
    }
}
