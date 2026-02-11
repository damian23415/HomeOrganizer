using FluentValidation;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;

namespace HomeOrganizer.Application.Features.WorkTracking.DTOs.Validators;

public class CreateHourlyRateRequestValidator : AbstractValidator<CreateHourlyRateRequest>
{
  public CreateHourlyRateRequestValidator()
  {
    RuleFor(x => x.Rate)
      .NotNull()
      .WithMessage("Hourly rate is required.")
      .GreaterThan(0)
      .WithMessage("Hourly rate must be greater than zero.");

    RuleFor(x => x.EffectiveFrom)
        .NotEmpty()
        .WithMessage("Effective from date is required.");
    
    RuleFor(x => x.EffectiveTo)
      .GreaterThan(x => x.EffectiveFrom)
      .When(x => x.EffectiveTo.HasValue)
      .WithMessage("Effective to date must be greater than effective from date.");
  }
}