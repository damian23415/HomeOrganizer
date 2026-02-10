using FluentValidation;

namespace HomeOrganizer.Application.Features.Users.DTOs.Validators;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
  public LoginUserRequestValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.");
        
    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
        .Matches(@"[A-Z]").WithMessage("Password must contain an uppercase letter.")
        .Matches(@"[a-z]").WithMessage("Password must contain a lowercase letter.")
        .Matches(@"\d").WithMessage("Password must contain a number.");
  }
}