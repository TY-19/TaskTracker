using FluentValidation.Results;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IValidationService
{
    public ValidationResult Validate(IValidatableModel model);
}
