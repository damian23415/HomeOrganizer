using FluentValidation;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;

namespace HomeOrganizer.Application.Features.WorkTracking.Validators;

public class WorkDayValidator : AbstractValidator<CreateWorkDayRequest>
{
  public WorkDayValidator()
  {
    RuleFor(x => x.StartTime)
      .NotEmpty().WithMessage("Godzina rozpoczęcia jest wymagana");

    RuleFor(x => x.EndTime)
      .NotEmpty().WithMessage("Godzina zakończenia jest wymagana")
      .GreaterThan(x => x.StartTime).WithMessage("Godzina zakończenia musi być po godzinie rozpoczęcia");

    RuleFor(x => x.EndTime.Subtract(x.StartTime).TotalHours)
      .GreaterThan(0).WithMessage("Dzień pracy musi trwać co najmniej kilka minut")
      .LessThanOrEqualTo(24).WithMessage("Dzień pracy nie może przekraczać 24 godzin");
  }
}