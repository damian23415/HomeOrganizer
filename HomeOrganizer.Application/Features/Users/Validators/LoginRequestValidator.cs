using FluentValidation;
using HomeOrganizer.Application.Features.Users.Dtos;

namespace HomeOrganizer.Application.Features.Users.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email jest wymagany.")
      .EmailAddress().WithMessage("Nieprawidłowy format adresu email.");
    
    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Hasło jest wymagane.")
      .Must(p => !string.IsNullOrWhiteSpace(p)).WithMessage("hasło nie może składać się tylko ze spacji.")
      .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków.")
      .MaximumLength(128).WithMessage("Hasło jest za długie");
  }
}