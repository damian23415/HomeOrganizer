using FluentValidation;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;

namespace HomeOrganizer.Application.Features.WorkTracking.Validators;

public class HourlyRateValidator : AbstractValidator<CreateHourlyRateRequest>
{
  public HourlyRateValidator()
  {
    RuleFor(x => x.HourlyRate)
      .GreaterThan(0)
      .WithMessage("Stawka godzinowa musi być większa od zera.");
    
    RuleFor(x => x.StartDate)
      .NotEmpty()
      .WithMessage("Data rozpoczęcia jest wymagana.")
      .LessThanOrEqualTo(DateTime.UtcNow)
      .WithMessage("Data rozpoczęcia nie może być w przyszłości.");
    
    RuleFor(x => x.EndDate)
      .GreaterThan(x => x.StartDate)
      .When(x => x.EndDate.HasValue)
      .WithMessage("Data zakończenia musi być późniejsza niż data rozpoczęcia.");
  }
}