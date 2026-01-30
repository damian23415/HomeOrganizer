using FluentValidation;
using HomeOrganizer.Application.Features.Users.Dtos;

namespace HomeOrganizer.Application.Features.Users.Validators;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
  public RegisterUserRequestValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress()
      .WithMessage("Nieprwidłowy format adresu email.");

    RuleFor(x => x.Password)
      .NotEmpty()
      .MinimumLength(8)
      .WithMessage("Hasło musi mieć minimum 8 znaków.");
  }
}