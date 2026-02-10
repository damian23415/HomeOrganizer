using FluentValidation;

namespace HomeOrganizer.Application.Features.Users.DTOs;

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
  public LoginUserValidator()
  {
    RuleFor(x => x.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
  }
}