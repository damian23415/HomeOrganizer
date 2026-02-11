using FluentValidation;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;

namespace HomeOrganizer.Application.Features.WorkTracking.DTOs.Validators;

public class CreateWorkDayRequestValidator : AbstractValidator<CreateWorkDayRequest>
{
  public CreateWorkDayRequestValidator()
  {
    RuleFor(x => x.StartTime)
        .NotEmpty().WithMessage("Start date is required");

    RuleFor(x => x.EndTime)
        .NotEmpty().WithMessage("End date is required")
        .GreaterThan(x => x.StartTime).WithMessage("Start hour must be before end hour");

    RuleFor(x => x.EndTime.Subtract(x.StartTime).TotalHours)
        .GreaterThan(0).WithMessage("Work day must be greater than 0 hours")
        .LessThanOrEqualTo(24).WithMessage("Work day cannot be greater than 24 hours");
  }
}