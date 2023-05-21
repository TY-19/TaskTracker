using FluentValidation.Results;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.Services;

public class ValidationService : IValidationService
{
    public ValidationResult Validate(IValidatableModel model)
    {
        var validator = ConfigureValidators.GetValidatorFor(model.GetType());
        return ((dynamic)validator).Validate((dynamic)model);
    }
}
