using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class EmployeePostModelValidator : AbstractValidator<EmployeePostModel>
{
    public EmployeePostModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Last name is required");
    }
}
