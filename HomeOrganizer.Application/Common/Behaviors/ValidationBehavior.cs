using FluentValidation;
using FluentValidation.Results;

namespace HomeOrganizer.Application.Common.Behaviors;

public class ValidationBehavior<T>(IValidator<T> validator)
{
  private readonly IValidator<T> _validator = validator;

  public async Task<ValidationResult> ValidateAsync(T request)
  {
    return await _validator.ValidateAsync(request);
  }
}