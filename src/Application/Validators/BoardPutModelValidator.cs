using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class BoardPutModelValidator : AbstractValidator<BoardPutModel>
{
    public BoardPutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Board's name cannot be an empty string");
        RuleFor(x => x.Name).MaximumLength(3)
            .WithMessage("Board's name must be at least 3 character long");
        RuleFor(x => x.Name).Custom((name, context) => {
            if (int.TryParse(name, out _))
                context.AddFailure("Board's name cannot contain only digits");
        });
    }
}
